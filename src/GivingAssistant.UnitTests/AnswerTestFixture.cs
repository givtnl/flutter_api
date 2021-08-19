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
using GivingAssistant.UnitTests.Infrastructure;
using NUnit.Framework;

namespace GivingAssistant.UnitTests
{
    [TestFixture]
    public class AnswerTestFixture : BaseTestFixture
    {

        [Test]
        [TestCase("My-Question-Id", "My-User-Id", "My-Answer")]
        public async Task EnsureAnswerIsInserted(string questionId, string userId, string answerId)
        {
            var commandHandler = new CreateAnswerCommandHandler(DynamoDb, Mapper);
            await commandHandler.Handle(new CreateAnswerCommand
            {
                Answer = answerId,
                QuestionId = questionId,
                UserId = userId
            }, CancellationToken.None);

            var dynamoDbResponse = await DynamoDb.LoadAsync<BaseItem>($"{Constants.UserPlaceholder}#{userId}",
                $"{Constants.AnswerPlaceholder}#{Constants.QuestionPlaceholder}#{questionId}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.IsNotNull(dynamoDbResponse);
        }
    }
}