using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Matches.Infrastructure
{
    public class MatchingRequest
    {
        public IEnumerable<UserTagMatchListModel> UserMatches { get; set; }
        public IEnumerable<UserCategoryMatchListModel> UserCategories { get; set; }
        public IEnumerable<OrganisationTagMatchListModel> OrganisationMatchesByTags { get; set; }
        public IEnumerable<OrganisationCategoryMatchListModel> OrganisationMatchesByCategories { get; set; }
    }
}