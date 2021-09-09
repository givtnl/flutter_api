using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Organisations;
using GivingAssistant.Business.Organisations.Commands.Create;
using Microsoft.AspNetCore.Http;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("[controller]")]
    public class OrganisationsController : BaseController
    {
        [HttpPost]
        [OpenApiOperation("CreateOrganisation", "Creates an organisation", "Creates a organisation for the user to donate to")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateOrganisationResponse))]
        public async Task<ActionResult<CreateOrganisationResponse>> Post([FromBody] CreateOrganisationRequest request, CancellationToken cancellationToken)
        => StatusCode(201, await Execute<CreateOrganisationRequest, CreateOrganisationResponse, CreateOrganisationCommand>(request, cancellationToken));

    }
}
