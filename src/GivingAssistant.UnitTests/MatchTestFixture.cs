using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch;
using GivingAssistant.Business.Matches.Commands.CreateUserTagMatch;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Persistence;
using GivingAssistant.UnitTests.Infrastructure;
using NUnit.Framework;

namespace GivingAssistant.UnitTests
{
    [TestFixture]
    public class MatchTestFixture : BaseTestFixture
    {
        [Test]
        [TestCase("Anthony", "animals", 0.75, 100)]
        public async Task EnsureUserMatchesArePersisted(string userId, string tag, decimal answer, int tagScore)
        {
            var commandHandler = new CreateUserTagMatchCommandHandler(DynamoDb, Mapper);
            await commandHandler.Handle(new CreateUserTagMatchCommand
            {
                User = userId,
                Answer = answer,
                Question = new QuestionTagListModel
                {
                    Tag = tag,
                    Score = tagScore
                }
            }, CancellationToken.None);

            var dynamoDbResponse = await DynamoDb.LoadAsync<UserTagMatch>($"{Constants.UserPlaceholder}#{userId}",
                $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{tag}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            Assert.AreEqual(answer * 100, dynamoDbResponse.Percentage);
        }
    }
}