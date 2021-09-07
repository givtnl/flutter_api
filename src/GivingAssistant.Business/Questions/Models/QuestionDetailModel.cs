using GivingAssistant.Domain;

namespace GivingAssistant.Business.Questions.Models
{
    public class QuestionDetailModel
    {
        public string Id { get; set; }
        public QuestionType Type { get; set; }
    }
}