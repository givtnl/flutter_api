using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;
using MediatR;

namespace GivingAssistant.Business.Organisations.Commands.Create
{
    public class CreateOrganisationCommand : IRequest<OrganisationDetailModel>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Mission { get; set; }
        public string Vision { get; set; }
        public string GivtIdentifier { get; set; }
        public Dictionary<string, string> MetaTags { get; set; }
        public Dictionary<string, int> TagScores { get; set; }
    }
}