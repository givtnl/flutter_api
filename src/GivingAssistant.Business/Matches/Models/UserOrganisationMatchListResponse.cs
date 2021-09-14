using System.Collections.Generic;

namespace GivingAssistant.Business.Matches.Models
{
    public class UserOrganisationMatchListResponse
    {
        public UserOrganisationMatchListResponse(IEnumerable<UserOrganisationMatchListModel> results, string nextPageToken)
        {
            Results = results;
            NextPageToken = nextPageToken;
        }
        public IEnumerable<UserOrganisationMatchListModel> Results { get; }
        public string  NextPageToken { get;  }
    }
}