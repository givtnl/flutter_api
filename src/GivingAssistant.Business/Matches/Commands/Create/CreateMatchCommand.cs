using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.Create
{
    public class CreateMatchCommand : IRequest
    {
        public string User { get; set; }
        public IEnumerable<OrganisationTagMatchListModel> MatchingOrganisations { get; set; }
    }
}
