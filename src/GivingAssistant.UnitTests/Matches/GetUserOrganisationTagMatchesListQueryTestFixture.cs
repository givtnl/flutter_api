using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Business.Matches.Queries.GetUserOrganisationTagMatchesList;
using GivingAssistant.UnitTests.Infrastructure;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.Matches
{
    [TestFixture]
    public class GetUserOrganisationTagMatchesListQueryTestFixture : BaseTestFixture
    {
        [Test]
        [TestCase("Anthony","74a4e206-8700-4db0-9f06-9edb270676d4",3)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnOrganisationId(string userId,string organisationId, int numberOfMatches)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationTagMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationTagMatchesListQuery {UserId = userId, OrganisationId = organisationId};
            var queryHandler = new GetUserOrganisationTagMatchesListQueryHandler(DynamoDb, Mapper);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.IsTrue(response.All(x => x.OrganisationId.Equals(organisationId)));
        }
        [Test]
        [TestCase("Anthony","74a4e206-8700-4db0-9f06-9edb270676d4",3)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnUserId(string userId,string organisationId, int numberOfMatches)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationTagMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationTagMatchesListQuery {UserId = userId, OrganisationId = organisationId};
            var queryHandler = new GetUserOrganisationTagMatchesListQueryHandler(DynamoDb, Mapper);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.AreEqual(numberOfMatches, response.Count());
        }
    }
}