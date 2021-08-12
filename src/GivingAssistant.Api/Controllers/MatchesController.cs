using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Matches;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithOrganisationsList;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("[controller]")]
    public class MatchesController : BaseController
    {
        [HttpGet]
        [OpenApiOperation("GetMatchesList", "Returns a list of matches", "Returns a list of matches to build a wall for the user")]
        public Task<GetMatchesListResponse> Get([FromQuery] GetMatchesListRequest request, CancellationToken cancellationToken)
        => Execute<GetMatchesListRequest, GetMatchesListResponse, GetMatchesWithOrganisationsListQuery>(request, cancellationToken);
    }
}