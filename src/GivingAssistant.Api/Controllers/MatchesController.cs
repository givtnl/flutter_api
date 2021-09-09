using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Matches;
using GivingAssistant.Business.Matches.Queries.GetUserOrganisationMatchesList;
using GivingAssistant.Business.Matches.Queries.GetUserTagMatchesList;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("[controller]")]
    public class MatchesController : BaseController
    {
        [HttpGet]
        [OpenApiOperation("GetMatchesList", "Returns a list of matches", "Returns a list of matches to build a wall for the user")]
        public Task<GetUserOrganisationMatchesListResponse> Get([FromQuery] GetUserOrganisationMatchesListRequest request, CancellationToken cancellationToken)
            => Execute<GetUserOrganisationMatchesListRequest, GetUserOrganisationMatchesListResponse, GetUserOrganisationMatchesListQuery>(request, cancellationToken);
    }
}