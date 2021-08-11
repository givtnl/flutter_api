using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests.Questions;
using GivingAssistant.Business.Answers.Commands.Create;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("questions/{questionId}/answers")]
    public class AnswersController : BaseController
    {

        [HttpPost]
        [OpenApiOperation("CreateAnswer", "Answers a question", "Registers an answer for a given question for a particular user")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateQuestionResponse))]
        public Task<CreateAnswerResponse> Post(string questionId,[FromBody] CreateAnswerRequest request, CancellationToken cancellationToken)
        {
            request.QuestionId = questionId;
            return Execute<CreateAnswerRequest, CreateAnswerResponse, CreateAnswerCommand>(request, cancellationToken);
        }
    }
}