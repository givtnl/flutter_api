namespace GivingAssistant.Business.Organisations.Models
{
    public class OrganisationCategoryMatchListModel
    {
        public string OrganisationId { get; set; }
        public string Category { get; set; }
        public OrganisationDetailModel Organisation { get; set; }
    }
}