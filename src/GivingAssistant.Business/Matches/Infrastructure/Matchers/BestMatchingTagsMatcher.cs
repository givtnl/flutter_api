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
        
        public Task<MatchingResponse> CalculateMatch(MatchingRequest context, CancellationToken token)
        {
            if (!context.OrganisationMatches.Any())
                return Task.FromResult(MatchingResponse.EmptyMatch());

            if (!context.UserMatches.Any())
                return Task.FromResult(MatchingResponse.EmptyMatch());

            var combinedMatches = context.UserMatches.Where(userMatch =>
                    context.OrganisationMatches.Any(organisationMatch =>
                        organisationMatch.Tag.Equals(userMatch.Tag, StringComparison.InvariantCultureIgnoreCase)))
                .OrderByDescending(x => x.Percentage)
                .Take(NumberOfTagsToEvaluate);


            var evaluatedScores = new List<decimal>();

            foreach (var match in combinedMatches)
            {
                // get the corresponding organisation match
                var organisationMatch = context.OrganisationMatches.FirstOrDefault(x => x.Tag.Equals(match.Tag, StringComparison.InvariantCultureIgnoreCase));
                if (organisationMatch == null)
                {
                    // this should really not occur, but safety first, zeker safety first
                    continue;
                }

                if (organisationMatch.Score > match.Percentage)
                    evaluatedScores.Add(match.Percentage / organisationMatch.Score * 100);
                else
                    evaluatedScores.Add(organisationMatch.Score / match.Percentage * 100);
            }

            return Task.FromResult(new MatchingResponse(decimal.Round(evaluatedScores.Average(), 2)));
        }
    }
}