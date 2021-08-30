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
            if (!context.OrganisationMatches.Any())
                return Task.FromResult(MatchingResponse.EmptyMatch());

            if (!context.UserMatches.Any())
                return Task.FromResult(MatchingResponse.EmptyMatch());
            
            // 7
            var numberOfTotalUserMatches = context.UserMatches.Count();
            // 3
            var numberOfMatchesBetweenUserAndOrganisation = context.UserMatches.Count(userMatch =>
                context.OrganisationMatches.Any(organisationMatch =>
                    organisationMatch.Tag.Equals(userMatch.Tag, StringComparison.InvariantCultureIgnoreCase)));

            var score = ((decimal)numberOfMatchesBetweenUserAndOrganisation / (decimal)numberOfTotalUserMatches) * 100;
            return Task.FromResult(new MatchingResponse(decimal.Round(score,2)));
        }
    }
}