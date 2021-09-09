using System.Collections.Generic;

namespace GivingAssistant.Business.Matches.Infrastructure
{
    public interface IUserOrganisationMatcher
    {
        int Order { get; }
        IEnumerable<MatchingResponse> CalculateMatches(MatchingRequest context, IEnumerable<MatchingResponse> currentResponses);
    }
}