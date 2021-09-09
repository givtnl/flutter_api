using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;

namespace GivingAssistant.Api.Requests.Matches
{
    public class GetUserOrganisationTagMatchesListResponse
    {
        public IEnumerable<UserOrganisationTagMatchListModel> Result { get; set; }
    }
}