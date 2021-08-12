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

        //[HttpGet]
        //[OpenApiOperation("GetOrganisaList", "Returns a list of questions", "Returns a list of question to build a profile for the user by answering them")]
        //public Task<GetQuestionsListResponse> Get([FromQuery] GetQuestionsListRequest request, CancellationToken cancellationToken)
        //=> Execute<GetQuestionsListRequest, GetQuestionsListResponse, GetQuestionsListQuery>(request, cancellationToken);


        [HttpPost]
        [OpenApiOperation("CreateOrganisation", "Creates an organisation", "Creates a organisation for the user to donate to")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateOrganisationResponse))]
        public async Task<ActionResult<CreateOrganisationResponse>> Post([FromBody] CreateOrganisationRequest request, CancellationToken cancellationToken)
        => StatusCode(201, await Execute<CreateOrganisationRequest, CreateOrganisationResponse, CreateOrganisationCommand>(request, cancellationToken));

    }
}
