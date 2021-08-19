namespace GivingAssistant.Persistence
{
    public class UserTagMatch : BaseItem
    {
        public string User { get; set; }
        public string Tag { get; set; }
        public int MaximumScore { get; set; }
        public int CurrentScore { get; set; }
        public decimal Percentage { get; set; }
    }
}