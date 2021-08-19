using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch
{
    public class CreateUserOrganisationMatchCommand : IRequest
    {
        public string User { get; set; }
        public IEnumerable<OrganisationTagMatchListModel> MatchingOrganisations { get; set; }
    }
}
