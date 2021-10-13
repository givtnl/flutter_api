using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Organisations;
using GivingAssistant.Business.Matches.Queries.GetUserOrganisationTagMatchesList;
using GivingAssistant.Business.Organisations.Queries.GetOrganisationTags;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("organisations/{organisationId:minlength(1)}/tags")]
    public class OrganisationTagsController : BaseController
    {
        [HttpGet]
        [OpenApiOperation("GetOrganisationTags", "Retrieves tags from the given organisation", "Retrieves all the tags and the scores for a given organisation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrganisationTagsResponse))]
        public async Task<ActionResult<GetOrganisationTagsResponse>> Get(string organisationId, CancellationToken cancellationToken)
        {
            var query = new GetOrganisationTagsRequest {OrganisationId = organisationId};
            return await Execute<GetOrganisationTagsRequest, GetOrganisationTagsResponse, GetOrganisationTagsQuery>(query, cancellationToken);
        }
    }
}