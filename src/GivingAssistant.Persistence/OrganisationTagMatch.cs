namespace GivingAssistant.Persistence
{
    public class OrganisationTagMatch : BaseItem
    {
        public OrganisationTagMatch()
        {
            
        }
        public OrganisationTagMatch(string organisationId, OrganisationProfile organisation)
        {
            OrganisationId = organisationId;
            Organisation = organisation;
        }
        public string OrganisationId { get; set; }
        public string Tag { get; set; }
        public int Score { get; set; }
        public OrganisationProfile Organisation { get; set; }
    }
}