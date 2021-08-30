namespace GivingAssistant.Business.Matches.Models
{
    public class UserTagMatchListModel
    {
        public string User { get; set; }
        public string Tag { get; set; }
        public int MaximumScore { get; set; }
        public int CurrentScore { get; set; }
        public decimal Percentage { get; set; }
    }
}