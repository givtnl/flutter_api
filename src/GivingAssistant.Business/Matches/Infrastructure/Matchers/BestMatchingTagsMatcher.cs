using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GivingAssistant.Business.Matches.Infrastructure.Matchers
{
    public class BestMatchingTagsMatcher : IUserOrganisationMatcher
    {
        private const int NumberOfTagsToEvaluate = 3;

        public int Order => 0;

        public IEnumerable<MatchingResponse> CalculateMatches(MatchingRequest context, IEnumerable<MatchingResponse> currentResponses)
        {
            if (!context.OrganisationMatchesByTags.Any())
                yield break;

            if (!context.UserMatches.Any())
                yield break;

            var combinedMatches = context.UserMatches.Where(userMatch =>
                    context.OrganisationMatchesByTags.Any(organisationMatch =>
                        organisationMatch.Tag.Equals(userMatch.Tag, StringComparison.InvariantCultureIgnoreCase)))
                .OrderByDescending(x => x.Percentage)
                .Take(NumberOfTagsToEvaluate);


            foreach (var match in combinedMatches)
            {
                // get the corresponding organisation match
                var organisationMatch = context.OrganisationMatchesByTags.FirstOrDefault(x => x.Tag.Equals(match.Tag, StringComparison.InvariantCultureIgnoreCase));
                if (organisationMatch == null)
                {
                    // this should really not occur, but safety first, zeker safety first
                    continue;
                }

                decimal score;

                if (organisationMatch.Score > match.Percentage)
                    score = match.Percentage / organisationMatch.Score * 100;
                else
                    score = organisationMatch.Score / match.Percentage * 100;

                yield return new MatchingResponse(score, match.Tag);
            }
        }
    }
}