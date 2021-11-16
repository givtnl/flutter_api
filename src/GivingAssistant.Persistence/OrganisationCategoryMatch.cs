namespace GivingAssistant.Persistence
{
    public class OrganisationCategoryMatch
    {
        public string OrganisationId { get; set; }
        public string Category { get; set; }
        public OrganisationProfile Organisation { get; set; }
    }
}