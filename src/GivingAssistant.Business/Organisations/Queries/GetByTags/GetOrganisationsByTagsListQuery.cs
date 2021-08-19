using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;
using MediatR;

namespace GivingAssistant.Business.Organisations.Queries.GetByTags
{
    public class GetOrganisationsByTagsListQuery : IRequest<IEnumerable<OrganisationTagMatchListModel>>
    {
        public IEnumerable<string> Tags { get; set; }
    }
}
