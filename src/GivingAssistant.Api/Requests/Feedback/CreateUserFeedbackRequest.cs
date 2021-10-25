using GivingAssistant.Business.Feedback;
using MediatR;

namespace GivingAssistant.Api.Requests
{
    public class CreateUserFeedbackRequest
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public UserFeedback UserFeedback { get; set; }
    }
}