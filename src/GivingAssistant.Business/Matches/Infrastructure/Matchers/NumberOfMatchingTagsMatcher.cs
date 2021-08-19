using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GivingAssistant.Business.Matches.Infrastructure.Matchers
{
    public class NumberOfMatchingTagsMatcher : IUserOrganisationMatcher
    {
        public Task<MatchingResponse> CalculateMatch(MatchingRequest context, CancellationToken token)
        {
            return Task.FromResult(new MatchingResponse
            {
                // ReSharper disable once PossibleLossOfFraction
                Score = context.OrganisationMatches.Count(x => context.UserMatches.Any(userMatch =>
                            userMatch.Tag.Equals(x.Tag, StringComparison.InvariantCultureIgnoreCase))) /
                        context.UserMatches.Count
            });

        }
    }
}