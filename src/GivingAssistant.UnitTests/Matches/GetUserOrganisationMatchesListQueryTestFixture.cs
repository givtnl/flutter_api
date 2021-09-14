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
        [TestCase("Anthony", 3)]
        public async Task EnsureOrganisationTagMatchesArePagesAreReturnedWithToken(string userId, int numberOfMatches)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId, Limit = 1, MinimumScore = 0};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper, DynamoDbClient);
            var firstPagedResponse = await queryHandler.Handle(query, CancellationToken.None);


            query.NextPageToken = firstPagedResponse.NextPageToken;
            var secondPagedResponse = await queryHandler.Handle(query, CancellationToken.None);
            Assert.AreNotEqual(firstPagedResponse.Results.FirstOrDefault()?.Organisation.Id, secondPagedResponse.Results.FirstOrDefault()?.Organisation.Id);
        }

        [Test]
        [TestCase("Anthony", 3)]
        public async Task EnsureOrganisationTagMatchesArePagesCorrectly(string userId, int numberOfMatches)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId, Limit = 1, MinimumScore = 0};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper, DynamoDbClient);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.NotNull(response.NextPageToken);
        }

        [Test]
        [TestCase("Anthony", 3)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnUserId(string userId, int numberOfMatches)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId, Limit = 100, MinimumScore = 0};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper, DynamoDbClient);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.AreEqual(numberOfMatches, response.Results.Count());
        }

        [Test]
        [TestCase("Anthony", 1, 85)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnMinimumScore(string userId, int numberOfMatches, int minimumScore)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId, Limit = 100, MinimumScore = minimumScore};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper, DynamoDbClient);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.IsTrue(response.Results.All(x => x.Score >= minimumScore));
            Assert.AreEqual(numberOfMatches, response.Results.Count());
        }

        [Test]
        [TestCase("Anthony", 2, 80)]
        public async Task EnsureOrganisationTagMatchesAreFilteredCorrectlyOnMinimumScoreWithPaging(string userId, int numberOfMatches, int minimumScore)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId, Limit = 3, MinimumScore = minimumScore};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper, DynamoDbClient);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.IsTrue(response.Results.All(x => x.Score >= minimumScore));
            Assert.AreEqual(numberOfMatches, response.Results.Count());
        }

        [Test]
        [TestCase("Anthony", 89.5)]
        public async Task EnsureOrganisationTagMatchesAreSortedCorrectly(string userId, decimal highestScore)
        {
            await Seed(Path.Combine(TestContext.CurrentContext.TestDirectory, "Matches", $"{nameof(GetUserOrganisationMatchesListQueryTestFixture)}.json"));
            var query = new GetUserOrganisationMatchesListQuery {UserId = userId, Limit = 100, MinimumScore = 100};
            var queryHandler = new GetUserOrganisationMatchesListQueryHandler(DynamoDb, Mapper, DynamoDbClient);

            var response = await queryHandler.Handle(query, CancellationToken.None);
            Assert.AreEqual(highestScore, response.Results.First().Score);
        }
    }
}