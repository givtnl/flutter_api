using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Business.Questions.Queries.GetList;
using GivingAssistant.Domain;
using GivingAssistant.Persistence;
using NUnit.Framework;

namespace GivingAssistant.UnitTests
{
    [TestFixture]
    public class QuestionTestFixture
    {
        private IDynamoDBContext _dynamoDb;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _dynamoDb = new DynamoDBContext(new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            }));
            _mapper = new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();

        }
        [TearDown]
        public async Task ClearDatabase()
        {
            var toDeleteItems = await _dynamoDb.ScanAsync<BaseItem>(Enumerable.Empty<ScanCondition>(),
                new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                }).GetRemainingAsync();

            foreach (var deleteItem in toDeleteItems)
            {
                await _dynamoDb.DeleteAsync(deleteItem, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }
        }
        [Test]
        public async Task EnsureQuestionIsGenerated()
        {
            var commandHandler = new CreateQuestionCommandHandler(_dynamoDb, _mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                TagScores = new Dictionary<string, int>
                {
                    {"Food", 100}
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);
           
            Assert.IsNotNull(response.Id);
        }
        [Test]
        public async Task EnsureQuestionsAreRetrievedFromList()
        {
            var commandHandler = new CreateQuestionCommandHandler(_dynamoDb, _mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                TagScores = new Dictionary<string, int>
                {
                    {"Food", 100}
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);

            var listResponse = await new GetQuestionsListQueryHandler(_dynamoDb, _mapper).Handle(new GetQuestionsListQuery(), CancellationToken.None);

            Assert.IsTrue(listResponse.Any(x => x.Id == response.Id) && listResponse.Count() == 1);
        }
        [Test]
        [TestCase("Food",60)]
        public async Task EnsureQuestionTagsAreInserted(string tagName, int score)
        {
            var commandHandler = new CreateQuestionCommandHandler(_dynamoDb, _mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                TagScores = new Dictionary<string, int>
                {
                    {tagName, score}
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);
            
            var dynamoDbResponse = await _dynamoDb.LoadAsync<BaseItem>(nameof(Question).ToUpper(),
                $"{response.Id}#{Constants.TagPlaceholder}#{tagName}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.IsNotNull(dynamoDbResponse);
        }
        [Test]
        [TestCase("Food", 60)]
        public async Task EnsureQuestionTagsCanBeRetrieved(string tagName, int score)
        {
            var commandHandler = new CreateQuestionCommandHandler(_dynamoDb, _mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                TagScores = new Dictionary<string, int>
                {
                    {tagName, score}
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);

            var dynamoDbResponse = await _dynamoDb.LoadAsync<BaseItem>(nameof(Question).ToUpper(),
                $"{response.Id}#{Constants.TagPlaceholder}#{tagName}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.IsNotNull(dynamoDbResponse);
        }
    }
}