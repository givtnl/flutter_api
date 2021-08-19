using System;
using System.Linq;
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
using GivingAssistant.Business.Organisations.Queries.GetByTags;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Business.Questions.Queries.GetTags;
using Microsoft.VisualBasic;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace GivingAssistant.UserMatchCalculator
{
    public class FunctionHandler
    {
        public IMapper Mapper { get; set; }
        public IDynamoDBContext DynamoDbContext { get; set; }
        public FunctionHandler()
        {
            Mapper ??= new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();
            DynamoDbContext ??= new DynamoDBContext(new AmazonDynamoDBClient());
        }

        public async Task HandleAsync(DynamoDBEvent @event, ILambdaContext lambdaContext)
        {
            lambdaContext.Logger.LogLine($"receiving event {@event.Records.Count}");
            foreach (var record in @event.Records)
            {
                var document = Document.FromAttributeMap(record.EventName == OperationType.REMOVE.Value ?
                    record.Dynamodb.OldImage : record.Dynamodb.NewImage);
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
                var questionIdentifier = sortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(2);

                var tagIdentifier = sortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3);

                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(questionIdentifier) || string.IsNullOrWhiteSpace(tagIdentifier))
                    continue;

                // get the tags that belong to this question
                var tagForQuestion = await
                    new GetQuestionTagsListQueryHandler(DynamoDbContext, Mapper)
                    .Handle(new GetQuestionTagsListQuery(questionIdentifier), CancellationToken.None);

                if (!tagForQuestion.Any())
                    continue;


                // calculate the user his or her score for the tags
                var createUserTagMatchCommandHandler = new CreateUserTagMatchCommandHandler(DynamoDbContext, Mapper);
                foreach (var questionTagListModel in tagForQuestion)
                {
                    await createUserTagMatchCommandHandler.Handle(new CreateUserTagMatchCommand
                        {
                            User = user,
                            Answer = document["SCORE"].AsInt(),
                            Question = questionTagListModel
                        }, CancellationToken.None)
                        ;
                }

                // get the organisations that belong to these tags
                var matchingOrganisations = await new GetOrganisationsByTagsListQueryHandler(DynamoDbContext, Mapper).Handle(
                    new GetOrganisationsByTagsListQuery
                    {
                        Tags = tagForQuestion.Select(x => x.Tag)
                    }, CancellationToken.None);


                ////TODO delete organisations that no longer match for this tag or update the scores

                if (!matchingOrganisations.Any())
                    continue;
                // TODO WORK IN PROGRESS
                // save the user matches
                await new CreateUserOrganisationMatchCommandHandler(DynamoDbContext, Mapper).Handle(new CreateUserOrganisationMatchCommand
                {
                    User = user,
                    MatchingOrganisations = matchingOrganisations
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
