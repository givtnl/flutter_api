using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Business.Matches.Infrastructure.Matchers;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.MatcherTestFixtures
{
    [TestFixture]
    public class NumberOfMatchingTagsMatcherTestFixture
    {
        private readonly NumberOfMatchingTagsMatcher _matcher = new();

        [Test]
        public async Task EnsureZeroIsReturnedWhenOrganisationHaveNoMatches()
        {
            var request = new MatchingRequest
            {
                OrganisationMatches = Enumerable.Empty<OrganisationTagMatchListModel>(),
                UserMatches = new[]
                {
                    new UserTagMatchListModel()
                    {
                        Percentage = 100,
                        Tag = "animals"
                    }
                }
            };
            var response = await _matcher.CalculateMatch(request, CancellationToken.None);
            Assert.AreEqual(decimal.Zero, response.Score);
        }

        [Test]
        public async Task EnsureMatchesAreCalculatedCorrectlyWhenUserHaveMoreMatchesThanOrganisation()
        {
            var request = new MatchingRequest
            {
                OrganisationMatches =new []
                {
                    new OrganisationTagMatchListModel()
                    {
                        Tag = "animals",
                        Score = 80
                    },
                    new OrganisationTagMatchListModel
                    {
                        Tag = "religion",
                        Score = 70
                    }
                },
                UserMatches = new[]
                {
                    new UserTagMatchListModel()
                    {
                        Percentage = 100,
                        Tag = "animals"
                    },
                    new UserTagMatchListModel
                    {
                        Tag = "religion",
                        Percentage = 70
                    },
                    new UserTagMatchListModel
                    {
                        Tag = "local",
                        Percentage = 70
                    },
                    new UserTagMatchListModel
                    {
                        Tag = "art",
                        Percentage = 70
                    }
                }
            };
            var response = await _matcher.CalculateMatch(request, CancellationToken.None);
            Assert.AreEqual(50m, response.Score);
        }
        
        [Test]
        public async Task EnsureZeroIsReturnedWhenUsersHaveNoMatches()
        {
            var request = new MatchingRequest
            {
                OrganisationMatches = new[]
                {
                    new OrganisationTagMatchListModel()
                    {
                        Score = 100,
                        Tag = "animals"
                    }
                },
                UserMatches = Enumerable.Empty<UserTagMatchListModel>()
            };
            var response = await _matcher.CalculateMatch(request, CancellationToken.None);
            Assert.AreEqual(decimal.Zero, response.Score);
        }
        
        [Test]
        public async Task EnsureMatchesAreCalculatedCorrectlyWhenOrganisationHaveMoreMatchesThanUser()
        {
            var request = new MatchingRequest
            {
                OrganisationMatches =new []
                {
                    new OrganisationTagMatchListModel
                    {
                        Tag = "animals",
                        Score = 80
                    },
                    new OrganisationTagMatchListModel
                    {
                        Tag = "religion",
                        Score = 70
                    },
                    new OrganisationTagMatchListModel
                    {
                        Tag = "international",
                        Score = 40
                    },
                    new OrganisationTagMatchListModel()
                    {
                        Tag = "art",
                        Score = 35
                    }
                },
                UserMatches = new[]
                {
                    new UserTagMatchListModel
                    {
                        Percentage = 100,
                        Tag = "animals"
                    },
                    new UserTagMatchListModel
                    {
                        Tag = "religion",
                        Percentage = 70
                    }
                }
            };
            var response = await _matcher.CalculateMatch(request, CancellationToken.None);
            Assert.AreEqual(100m, response.Score);
        }
    }
}