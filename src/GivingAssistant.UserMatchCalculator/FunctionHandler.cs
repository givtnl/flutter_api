using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using AutoMapper;
using GivingAssistant.Business.Matches.Commands.Create;
using GivingAssistant.Business.Organisations.Queries.GetByTags;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Business.Questions.Queries.GetTags;
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace GivingAssistant.UserMatchCalculator
{
    public class FunctionHandler
    {
        public IMapper Mapper { get; set; }
        public IAmazonDynamoDB DynamoDbClient { get; set; }
        public FunctionHandler()
        {
            Mapper ??= new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();
            DynamoDbClient ??= new AmazonDynamoDBClient();
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
                // example ANSWER#QUESTION#WhatIsTheMeaningLife
                var questionIdentifier = sortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(2);

                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(questionIdentifier))
                    continue;

                // get the tags that belong to this question
                var tagForQuestion = await
                    new GetQuestionTagsListQueryHandler(DynamoDbClient, Mapper)
                    .Handle(new GetQuestionTagsListQuery(questionIdentifier), CancellationToken.None);

                // get the organisations that belong to these tags
                var matchingOrganisations = await new GetOrganisationsByTagsListQueryHandler(DynamoDbClient, Mapper).Handle(
                    new GetOrganisationsByTagsListQuery
                    {
                        Tags = tagForQuestion.Select(x => x.Tag)
                    }, CancellationToken.None);

                // TODO WORK IN PROGRESS
                // save the user matches
                await new CreateMatchCommandHandler(DynamoDbClient, Mapper).Handle(new CreateMatchCommand
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
