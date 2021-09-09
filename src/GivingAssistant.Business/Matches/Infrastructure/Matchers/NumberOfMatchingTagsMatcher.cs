using System;
using System.Collections.Generic;
using System.Linq;

namespace GivingAssistant.Business.Matches.Infrastructure.Matchers
{
    public class NumberOfMatchingTagsMatcher : IUserOrganisationMatcher
    {
        public int Order => 1;
        
        public IEnumerable<MatchingResponse> CalculateMatches(MatchingRequest context, IEnumerable<MatchingResponse> currentResponses)
        {
            if (!context.OrganisationMatches.Any())
                yield break;

            if (!context.UserMatches.Any())
                yield break;
            
            // 7
            var numberOfTotalUserMatches = context.UserMatches.Count();
            // 3
            var numberOfMatchesBetweenUserAndOrganisation = context
                .UserMatches.Count(userMatch => context
                    .OrganisationMatches
                    .Any(organisationMatch => organisationMatch.Tag.Equals(userMatch.Tag, StringComparison.InvariantCultureIgnoreCase)));

            var score = ((decimal) numberOfMatchesBetweenUserAndOrganisation / (decimal) numberOfTotalUserMatches) * 100;

            foreach (var matching in currentResponses)
            {
                var recalculatedScore = decimal.Round((score + matching.Score) / 2, 2);
                yield return new MatchingResponse(recalculatedScore, matching.Tag);
            }
        }
    }
}