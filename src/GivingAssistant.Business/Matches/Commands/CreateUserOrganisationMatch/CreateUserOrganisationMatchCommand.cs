using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Business.Organisations.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch
{
    public class CreateUserOrganisationMatchCommand : IRequest
    {
        public string User { get; set; }
        public IEnumerable<UserTagMatchListModel> UserTags { get; set; }
        public IEnumerable<UserCategoryMatchListModel> UserCategories { get; set; }
        public IEnumerable<OrganisationTagMatchListModel> MatchingOrganisationsByTag { get; set; }
        public IEnumerable<OrganisationCategoryMatchListModel> MatchingOrganisationsByCategory { get; set; }
    }
}
