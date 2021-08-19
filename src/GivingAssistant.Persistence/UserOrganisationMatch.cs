namespace GivingAssistant.Persistence
{
    public class UserOrganisationMatch : BaseItem
    {
        public string Tag { get; set; }
        public OrganisationProfile Organisation { get; set; }
        public int Score { get; set; }
    }
}