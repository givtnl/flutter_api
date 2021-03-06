using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Questions;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Queries.GetList;
using Microsoft.AspNetCore.Http;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("[controller]")]
    public class QuestionsController : BaseController
    {

        [HttpGet]
        [OpenApiOperation("GetQuestionsList", "Returns a list of questions", "Returns a list of question to build a profile for the user by answering them")]
        public Task<GetQuestionsListResponse> Get(CancellationToken cancellationToken)
        => Execute<GetQuestionsListRequest, GetQuestionsListResponse, GetQuestionsListQuery>(new GetQuestionsListRequest(), cancellationToken);


        [HttpPost]
        [OpenApiOperation("CreateQuestion", "Creates a question", "Creates a question for the user to answer")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateQuestionResponse))]
        public async Task<ActionResult<CreateQuestionResponse>> Post([FromBody] CreateQuestionRequest request, CancellationToken cancellationToken)
        => StatusCode(201, await Execute<CreateQuestionRequest, CreateQuestionResponse, CreateQuestionCommand>(request, cancellationToken));

    }
}
