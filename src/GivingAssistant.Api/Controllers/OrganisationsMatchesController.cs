using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Matches;
using GivingAssistant.Business.Matches.Queries.GetUserOrganisationTagMatchesList;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("organisations/{organisationId}/matches")]
    public class OrganisationsMatchesController : BaseController
    {
        [HttpGet]
        [OpenApiOperation("GetUserOrganisationTagMatchesList", "Returns a list of matches for the combination between an user and an organisation", 
            "Returns a list of calculated matches between the profile of the user and the profile of the organisation")]
        public Task<GetUserOrganisationTagMatchesListResponse> Get(string organisationId,[FromQuery] GetUserOrganisationTagMatchesListRequest request, CancellationToken cancellationToken)
            => Execute<GetUserOrganisationTagMatchesListRequest, GetUserOrganisationTagMatchesListResponse, GetUserOrganisationTagMatchesListQuery>(request, cancellationToken);
    }
}