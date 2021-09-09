using System.Collections.Generic;
using GivingAssistant.Business.Matches.Infrastructure;

namespace GivingAssistant.UnitTests.Infrastructure
{
    public class FakeMatcher : IUserOrganisationMatcher
    {
        private readonly decimal _toMockScore;
        private readonly string _tag;

        public FakeMatcher(decimal toMockScore, string tag)
        {
            _toMockScore = toMockScore;
            _tag = tag;
        }

        public static IEnumerable<IUserOrganisationMatcher> BuildFakeWithScore(decimal score, string tag)
        {
            return new[] {new FakeMatcher(score, tag)};
        }

        public int Order { get; }

        public IEnumerable<MatchingResponse> CalculateMatches(MatchingRequest context, IEnumerable<MatchingResponse> currentResponses)
        {
            yield return new MatchingResponse(_toMockScore, _tag);
        }
    }
}