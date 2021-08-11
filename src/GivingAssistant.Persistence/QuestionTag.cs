namespace GivingAssistant.Persistence
{
    public class QuestionTag : BaseItem
    {
        public string QuestionId { get; set; }
        public int Score { get; set; }
        public string Tag { get; set; }

        public QuestionTag()
        {
            
        }
        public QuestionTag(string questionId)
        {
            QuestionId = questionId;
        }
    }
}