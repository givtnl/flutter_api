using MediatR;

namespace GivingAssistant.Business.Feedback
{
    public class CreateUserFeedbackCommand: IRequest
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public UserFeedback UserFeedback { get; set; }
    }
}