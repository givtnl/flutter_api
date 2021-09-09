using GivingAssistant.Business.Organisations.Models;

namespace GivingAssistant.Business.Matches.Models
{
    public class UserOrganisationMatchListModel
    {
        public OrganisationDetailModel Organisation { get; set; }
        public int Score { get; set; }
    }
}