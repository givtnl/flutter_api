using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;
using MediatR;

namespace GivingAssistant.Business.Organisations.Queries.GetOrganisationTags
{
    public class GetOrganisationTagsQuery : IRequest<IEnumerable<OrganisationTagMatchListModel>>
    {
        public string OrganisationId { get; set; }
    }
}