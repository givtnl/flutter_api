using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetUserCategoriesMatchesList
{
    public class GetUserCategoriesMatchesListQuery: IRequest<IEnumerable<UserCategoryMatchListModel>>
    {
        public string UserId { get; set; }
    }
}