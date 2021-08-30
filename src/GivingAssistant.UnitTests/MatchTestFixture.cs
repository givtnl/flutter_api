using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch;
using GivingAssistant.Business.Matches.Commands.CreateUserTagMatch;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithOrganisationsList;
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

        [Test]
        [TestCase("Anthony")]
        public async Task EnsureMatchesArePersisted(string userId)
        {
            var commandHandler = new CreateUserOrganisationMatchCommandHandler(DynamoDb, Mapper,FakeMatcher.BuildFakeWithScore(80));
            await commandHandler.Handle(new CreateUserOrganisationMatchCommand
            {
                User = userId,
                MatchingOrganisations = new[]{
                  new OrganisationTagMatchListModel
                  {
                      OrganisationId = "org-2",
                      Score = 80,
                      Tag = "black",
                      Organisation = new OrganisationDetailModel
                      {
                          Id = "org-2"

                      }
                  },
                  new OrganisationTagMatchListModel
              {
                  OrganisationId = "org-1",
                  Organisation = new OrganisationDetailModel
                  {
                      Id = "org-1"
                  },
                  Score = 20,
                  Tag = "yellow"
              }}
            }, CancellationToken.None);

            var response = await new GetMatchesWithOrganisationsListQueryHandler(DynamoDb, Mapper).Handle(new GetMatchesWithOrganisationsListQuery() { UserId = userId }, CancellationToken.None);

            Assert.IsTrue(response.Any(x => x.Organisation.Id == "org-1"));
            Assert.IsTrue(response.Any(x => x.Organisation.Id == "org-2"));
        }
    }
}