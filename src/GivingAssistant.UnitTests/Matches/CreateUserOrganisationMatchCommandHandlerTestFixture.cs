using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;
using GivingAssistant.UnitTests.Infrastructure;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.Matches
{
    [TestFixture]
    public class CreateUserOrganisationMatchCommandHandlerTestFixture : BaseTestFixture
    {
        [Test]
        [TestCase("Anthony", "animals", 80, "my-org-id")]
        public async Task EnsureTotalScoresArePersisted(string user, string tag, decimal score, string organisationId)
        {
            var commandHandler = new CreateUserOrganisationMatchCommandHandler(DynamoDb, Mapper, new[] {new FakeMatcher(80, "animals")});
            await commandHandler.Handle(new CreateUserOrganisationMatchCommand
            {
                User = user,
                MatchingOrganisations = new[] {new OrganisationTagMatchListModel {Tag = tag, Score = 80, OrganisationId = organisationId, Organisation = new OrganisationDetailModel{Id = organisationId}}},
                UserTags = new[] {new UserTagMatchListModel {Percentage = score, Tag = tag}}
            }, CancellationToken.None);

            var matches = await RetrieveRecords<UserOrganisationMatch>($"{Constants.UserPlaceholder}#{user}",
                $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TotalScorePlaceHolder}#{score}#{organisationId}");
            
            Assert.AreEqual(matches.ElementAtOrDefault(0)?.Score, score);
            Assert.AreEqual(matches.ElementAtOrDefault(0)?.Organisation.Id, organisationId);
        }
        
        [Test]
        [TestCase("Anthony", "animals", 80, "my-org-id")]
        public async Task EnsureIndividualTagsArePersisted(string user, string tag, decimal score, string organisationId)
        {
            var commandHandler = new CreateUserOrganisationMatchCommandHandler(DynamoDb, Mapper, new[] {new FakeMatcher(80, "animals")});
            await commandHandler.Handle(new CreateUserOrganisationMatchCommand
            {
                User = user,
                MatchingOrganisations = new[] {new OrganisationTagMatchListModel {Tag = tag, Score = 80, OrganisationId = organisationId, Organisation = new OrganisationDetailModel{Id = organisationId}}},
                UserTags = new[] {new UserTagMatchListModel {Percentage = score, Tag = tag}}
            }, CancellationToken.None);

            var matches = await RetrieveRecords<UserOrganisationTagMatch>($"{Constants.UserPlaceholder}#{user}",
                $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{organisationId}#{Constants.TagPlaceholder}#{tag}");
            
            Assert.AreEqual(matches.ElementAtOrDefault(0)?.Score, score);
        }
    }
}