using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;

namespace GivingAssistant.Api.Requests.Matches
{
    public class GetMatchesListResponse
    {
        public IEnumerable<UserOrganisationMatchListModel> Result { get; set; }
    }
}