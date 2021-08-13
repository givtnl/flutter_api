namespace GivingAssistant.Business.Organisations.Models
{
    public class OrganisationTagMatchListModel
    {
        public string OrganisationId { get; set; }
        public string Tag { get; set; }
        public int Score { get; set; }
        public OrganisationDetailModel Organisation { get; set; }
    }
}
