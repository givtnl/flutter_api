using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;

namespace GivingAssistant.Api.Requests.Matches
{
    public class GetMatchesListResponse
    {
        public IEnumerable<UserOrganisationMatchListModel> OrganisationMatches { get; set; }
        public IEnumerable<UserTagMatchListModel> UserTagMatches { get; set; }
    }
}