using System.Threading;
using System.Threading.Tasks;

namespace GivingAssistant.Business.Matches.Infrastructure
{
    public interface IUserOrganisationMatcher
    {
        Task<MatchingResponse> CalculateMatch(MatchingRequest context, CancellationToken token);
    }
}