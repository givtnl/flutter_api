using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Business.Matches.Queries.GetUserOrganisationMatchesList;
using GivingAssistant.UnitTests.Infrastructure;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.Matches
{
    [TestFixture]
    public class GetUserOrganisationMatchesListQueryTestFixture : BaseTestFixture
    {
        [Test]
        [TestCase("Anthony",3)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnUserId(string userId, int numberOfMatches)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId,Limit = 100, MinimumScore = 0};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.AreEqual(numberOfMatches, response.Count());
        }
        
        [Test]
        [TestCase("Anthony",1,85)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnMinimumScore(string userId, int numberOfMatches, int minimumScore)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId,Limit = 100, MinimumScore = minimumScore};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.IsTrue(response.All(x => x.Score >= minimumScore));
            Assert.AreEqual(numberOfMatches, response.Count());
        }
        
        [Test]
        [TestCase("Anthony",2,80)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnMinimumScoreWithPaging(string userId, int numberOfMatches, int minimumScore)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId,Limit = 3, MinimumScore = minimumScore};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.IsTrue(response.All(x => x.Score >= minimumScore));
            Assert.AreEqual(numberOfMatches, response.Count());
        }
        
        [Test]
        [TestCase("Anthony",89.5)]
        public async Task EnsureOrganisationTagMatchesAreSortedCorrectly(string userId, decimal highestScore)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId,Limit = 100, MinimumScore = 100};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.AreEqual(highestScore, response.First().Score );
        }
    }
}