using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;
using MediatR;

namespace GivingAssistant.Business.Organisations.Queries.GetByCategories
{
    public class GetOrganisationsByCategoriesListQuery: IRequest<IEnumerable<OrganisationCategoryMatchListModel>>
    {
        public IEnumerable<string> Categories { get; set; }
        
    }
}