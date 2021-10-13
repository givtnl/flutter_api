using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;

namespace GivingAssistant.Api.Requests.Organisations
{
    public class GetOrganisationTagsResponse
    {
        public IEnumerable<OrganisationTagMatchListModel> Results { get; set; }
    }
}