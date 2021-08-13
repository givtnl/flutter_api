namespace GivingAssistant.Persistence
{
    public class UserMatch : BaseItem
    {
        public string Tag { get; set; }
        public OrganisationProfile Organisation { get; set; }
        public int Score { get; set; }
    }
}