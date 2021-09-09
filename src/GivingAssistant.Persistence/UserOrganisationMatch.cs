namespace GivingAssistant.Persistence
{
    public class UserOrganisationMatch : BaseItem
    {
        public OrganisationProfile Organisation { get; set; }
        public decimal Score { get; set; }
    }

    public class UserOrganisationTagMatch : BaseItem
    {
        public decimal Score { get; set; }   
    }
}