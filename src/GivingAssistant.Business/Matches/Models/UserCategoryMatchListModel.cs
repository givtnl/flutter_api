namespace GivingAssistant.Business.Matches.Models
{
    public class UserCategoryMatchListModel
    {
        public string User { get; set; }
        public string Category { get; set; }
        public int MaximumScore { get; set; }
        public int CurrentScore { get; set; }
        public decimal Percentage { get; set; }
    }
}