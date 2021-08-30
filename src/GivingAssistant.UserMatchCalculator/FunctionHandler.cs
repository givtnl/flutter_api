using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using AutoMapper;
using GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch;
using GivingAssistant.Business.Matches.Commands.CreateUserTagMatch;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Business.Matches.Infrastructure.Matchers;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithTagsList;
using GivingAssistant.Business.Organisations.Queries.GetByTags;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Business.Questions.Queries.GetTags;
using Microsoft.VisualBasic;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GivingAssistant.UserMatchCalculator
{
    public class FunctionHandler
    {
        private IEnumerable<IUserOrganisationMatcher> MatchMakers { get; set; }
        public IMapper Mapper { get; set; }
        public IDynamoDBContext DynamoDbContext { get; set; }

        public FunctionHandler()
        {
            Mapper = new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();
            DynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            MatchMakers = new IUserOrganisationMatcher[]
            {
                new BestMatchingTagsMatcher(),
                new NumberOfMatchingTagsMatcher()
            };
        }

        public async Task HandleAsync(DynamoDBEvent @event, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine("receiving event");
            foreach (var record in @event.Records)
            {
                try
                {
                    var document = Document.FromAttributeMap(record.EventName == OperationType.REMOVE.Value
                        ? record.Dynamodb.OldImage
                        : record.Dynamodb.NewImage);
                    // if its not valid, then its for another stream listener to process
                    if (!IsValid(document))
                        continue;

                    // parse the question identifier and the user
                    var primaryKey = document["PK"].AsString();
                    var sortKey = document["SK"].AsString();

                    lambdaContext.Logger.LogLine($"Handling PK {primaryKey} & SK {sortKey}");

                    // example  USER#AnthonyBoetong
                    var user = primaryKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);
                    // example ANSWER#QUESTION#WhatIsTheMeaningLife#MyTag
                    var questionIdentifier =
                        sortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(2);

                    var tagIdentifier = sortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3);

                    if (string.IsNullOrWhiteSpace(user))
                    {
                        lambdaContext.Logger.LogLine($"User is empty (PK:{primaryKey})(SK:{sortKey})");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(questionIdentifier))
                    {
                        lambdaContext.Logger.LogLine($"Question is empty (PK:{primaryKey})(SK:{sortKey})");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(tagIdentifier))
                    {
                        lambdaContext.Logger.LogLine($"Tag is empty (PK:{primaryKey})(SK:{sortKey})");
                        continue;
                    }

                    // get the tags that belong to this question
                    var tagsForQuestion = await
                        new GetQuestionTagsListQueryHandler(DynamoDbContext, Mapper)
                            .Handle(new GetQuestionTagsListQuery(questionIdentifier), CancellationToken.None);

                    if (!tagsForQuestion.Any())
                    {
                        lambdaContext.Logger.LogLine($"No Tags for this question (PK:{primaryKey})(SK:{sortKey})");
                        continue;
                    }

                    var currentScoredAnswer = document["Score"].AsDecimal();
                    if (currentScoredAnswer <= 0)
                    {
                        lambdaContext.Logger.LogLine(
                            $"currentScoredAnswer for this question is zero (PK:{primaryKey})(SK:{sortKey})");
                        continue;
                    }

                    // calculate the user his or her score for the tags
                    await SaveAndCalculateUserTagMatches(lambdaContext, tagsForQuestion, user, currentScoredAnswer,
                        primaryKey, sortKey);

                    // get the organisations that belong to these tags
                    var matchingOrganisations =
                        await new GetOrganisationsByTagsListQueryHandler(DynamoDbContext, Mapper).Handle(
                            new GetOrganisationsByTagsListQuery
                            {
                                Tags = tagsForQuestion.Select(x => x.Tag)
                            }, CancellationToken.None);

                    ////TODO delete organisations that no longer match for this tag or update the scores
                    if (!matchingOrganisations.Any())
                    {
                        lambdaContext.Logger.LogLine(
                            $"No matching organisations for these Tags (PK:{primaryKey})(SK:{sortKey})");
                        continue;
                    }

                    // TODO WORK IN PROGRESS
                    // save the user matches
                    await new CreateUserOrganisationMatchCommandHandler(DynamoDbContext, Mapper, MatchMakers).Handle(
                        new CreateUserOrganisationMatchCommand
                        {
                            User = user,
                            MatchingOrganisations = matchingOrganisations,
                            UserTags = await new GetMatchesWithTagsListQueryHandler(DynamoDbContext, Mapper).Handle(
                                new GetMatchesWithTagsListQuery
                                {
                                    UserId = user
                                }, CancellationToken.None)
                        }, CancellationToken.None);
                }
                catch (Exception e)
                {
                    lambdaContext.Logger.LogLine(e.ToString());
                }
            }
        }

        private async Task SaveAndCalculateUserTagMatches(ILambdaContext lambdaContext,
            IEnumerable<QuestionTagListModel> tagsForQuestion, string user, decimal currentScoredAnswer,
            string primaryKey, string sortKey)
        {
            var createUserTagMatchCommandHandler = new CreateUserTagMatchCommandHandler(DynamoDbContext, Mapper);
            foreach (var questionTagListModel in tagsForQuestion)
            {
                lambdaContext.Logger.LogLine(
                    $"Creating Match for user {user} with tag {questionTagListModel.Tag}(QUESTIONTAGSCORE:{questionTagListModel.Score}) (ANSWER:{currentScoredAnswer})(PK:{primaryKey})(SK:{sortKey})");
                await createUserTagMatchCommandHandler.Handle(new CreateUserTagMatchCommand
                {
                    User = user,
                    Answer = currentScoredAnswer,
                    Question = questionTagListModel
                }, CancellationToken.None);
            }
        }

        private static bool IsValid(Document document)
        {
            return document.ContainsKey("SK") &&
                   document.ContainsKey("PK") &&
                   document["PK"].AsString().StartsWith("USER") &&
                   document["SK"].AsString().StartsWith("ANSWER#QUESTION");
        }
    }
}