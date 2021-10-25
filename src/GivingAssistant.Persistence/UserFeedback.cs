using GivingAssistant.Domain;

namespace GivingAssistant.Persistence
{
    public class UserFeedback: BaseItem
    {
        public string UserId { get; set; }
        public FeedbackScore Feedback { get; set; }
    }
}