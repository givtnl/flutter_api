using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests;
using GivingAssistant.Business.Answers.Commands.Create;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("users/{userid}/feedback")]
    public class FeedbackController: BaseController
    {
       
        [HttpPost]
        [OpenApiOperation("CreateAnswer", "Answers a question", "Registers an answer for a given question for a particular user")]
        public async Task<IActionResult> Post(string userId,[FromBody] CreateUserFeedbackRequest request, CancellationToken cancellationToken)
        {
            return StatusCode(201, await Execute<CreateUserFeedbackRequest, CreateUserFeedbackResponse, CreateAnswerCommand>(request, cancellationToken));
        }
    }
}