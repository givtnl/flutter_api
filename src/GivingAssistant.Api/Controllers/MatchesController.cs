using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Matches;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithOrganisationsList;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithTagsList;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("[controller]")]
    public class MatchesController : BaseController
    {
        [HttpGet]
        [OpenApiOperation("GetMatchesList", "Returns a list of matches", "Returns a list of matches to build a wall for the user")]
        public async Task<GetMatchesListResponse> Get([FromQuery] GetMatchesListRequest request, CancellationToken cancellationToken)
        {
            var getOrganisationMatchesRequest = new GetMatchesWithOrganisationsListQuery {UserId = request.UserId, MinimumScore = request.MinimumScore};
            var getUserTagMatchesRequest = new GetMatchesWithTagsListQuery {UserId = request.UserId};

            return new GetMatchesListResponse
            {
                OrganisationMatches = await Mediator.Send(getOrganisationMatchesRequest, cancellationToken),
                UserTagMatches = await Mediator.Send(getUserTagMatchesRequest, cancellationToken)
            };
        }
    }
}