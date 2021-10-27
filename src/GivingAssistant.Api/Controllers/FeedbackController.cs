using System.Threading;
using System.Threading.Tasks;
using GivingAssistant.Api.Requests;
using GivingAssistant.Business.Feedback;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace GivingAssistant.Api.Controllers
{
    [Route("users/{userId}/feedback")]
    public class FeedbackController: BaseController
    {
       
        [HttpPost]
        [OpenApiOperation("CreateFeedback", "Give feedback", "Registers feedback and the email from a specific user linked to an userid.")]
        public async Task<IActionResult> Post(string userId,[FromBody] CreateUserFeedbackRequest request, CancellationToken cancellationToken)
        {
            request.UserId = userId;
            return StatusCode(201, await Execute<CreateUserFeedbackRequest, CreateUserFeedbackResponse, CreateUserFeedbackCommand>(request, cancellationToken));
        }
    }
}