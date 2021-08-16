using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using GivingAssistant.Business.Answers.Commands.Create;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Persistence;
using NUnit.Framework;

namespace GivingAssistant.UnitTests
{
    [TestFixture]
    public class AnswerTestFixture
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
        [TestCase("My-Question-Id", "My-User-Id", "My-Answer")]
        public async Task EnsureAnswerIsInserted(string questionId, string userId, string answerId)
        {
            var commandHandler = new CreateAnswerCommandHandler(_dynamoDb, _mapper);
            await commandHandler.Handle(new CreateAnswerCommand
            {
                Answer = answerId,
                QuestionId = questionId,
                UserId = userId
            }, CancellationToken.None);

            var dynamoDbResponse = await _dynamoDb.LoadAsync<BaseItem>($"{Constants.UserPlaceholder}#{userId}",
                $"{Constants.AnswerPlaceholder}#{Constants.QuestionPlaceholder}#{questionId}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.IsNotNull(dynamoDbResponse);
        }
    }
}