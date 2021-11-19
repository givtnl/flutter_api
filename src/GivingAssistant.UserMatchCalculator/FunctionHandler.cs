using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using AutoMapper;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Business.Matches.Infrastructure.Matchers;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Business.Questions.Queries.GetDetail;
using GivingAssistant.Business.Questions.Queries.GetTags;
using GivingAssistant.UserMatchCalculator.Handlers;
using GivingAssistant.UserMatchCalculator.Models;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GivingAssistant.UserMatchCalculator
{
    public class FunctionHandler
    {
        public IEnumerable<IAnsweredQuestionHandler> Handlers { get; set; }
        public IEnumerable<IUserOrganisationMatcher> MatchMakers { get; set; }
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
            Handlers = new IAnsweredQuestionHandler[]
            {
                new CategoryAnsweredHandler(DynamoDbContext, Mapper),
                new StatementAnsweredHandler(DynamoDbContext, Mapper),
                new ReCalculateUserTagsAndOrganisationMatchHandler(DynamoDbContext, Mapper, MatchMakers),
                new DeleteCurrentTotalScoresHandler(DynamoDbContext, new AmazonDynamoDBClient())
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
                    if (!EnsureIsValid(document))
                        continue;

                    // parse the question identifier and the user
                    var primaryKey = document["PK"].AsString();
                    var sortKey = document["SK"].AsString();

                    var user = primaryKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);
                    var questionIdentifier = sortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(2);
                    var tagIdentifier = sortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3);

                    if (!EnsureIsValid(lambdaContext, user, questionIdentifier, tagIdentifier))
                        continue;

                    var request = new HandleAnsweredQuestionRequest
                    {
                        Answer = document["Score"].AsDecimal(),
                        User = user,
                        AnsweredTag = tagIdentifier,
                        LambdaContext = lambdaContext,
                        QuestionTags = 
                            await new GetQuestionTagsListQueryHandler(DynamoDbContext, Mapper).Handle(new GetQuestionTagsListQuery(questionIdentifier), CancellationToken.None),
                        AnsweredQuestion =
                            await new GetQuestionDetailQueryHandler(DynamoDbContext, Mapper).Handle(new GetQuestionDetailQuery {Id = questionIdentifier}, CancellationToken.None)
                    };

                    // find the handlers that can run this message
                    foreach (var handler in Handlers.Where(x => x.CanHandle(request)).OrderBy(x => x.ExecutionOrder))
                    {
                        await handler.Handle(request);
                    }
                }
                catch (Exception e)
                {
                    lambdaContext.Logger.LogLine(e.ToString());
                }
            }
        }


        private static bool EnsureIsValid(Document document)
        {
            return document.ContainsKey("SK") &&
                   document.ContainsKey("PK") &&
                   document["PK"].AsString().StartsWith("USER") &&
                   document["SK"].AsString().StartsWith("ANSWER#QUESTION");
        }

        private static bool EnsureIsValid(ILambdaContext lambdaContext, params string[] payloads)
        {
            if (!payloads.Any(string.IsNullOrWhiteSpace))
                return true;

            lambdaContext.Logger.LogLine($"Invalid Message ({string.Join(",", payloads)}");
            return false;
        }
    }
}