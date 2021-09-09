namespace GivingAssistant.Business.Matches.Models
{
    public class UserOrganisationTagMatchListModel
    {
        public string OrganisationId { get; set; }
        public string Tag { get; set; }
        public decimal Score { get; set; }
    }
}