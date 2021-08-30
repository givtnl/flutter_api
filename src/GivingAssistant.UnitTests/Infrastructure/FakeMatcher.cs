using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Business.Matches.Infrastructure;

namespace GivingAssistant.UnitTests.Infrastructure
{
    public class FakeMatcher : IUserOrganisationMatcher
    {
        private readonly decimal _toMockScore;

        public FakeMatcher(decimal toMockScore)
        {
            _toMockScore = toMockScore;
        }
        public Task<MatchingResponse> CalculateMatch(MatchingRequest context, CancellationToken token)
        {
            return Task.FromResult(new MatchingResponse(_toMockScore));
        }

        public static IEnumerable<IUserOrganisationMatcher> BuildFakeWithScore(decimal score)
        {
            return new[] {new FakeMatcher(score)};
        }
    }
    
    }