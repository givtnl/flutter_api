using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetUserOrganisationTagMatchesList
{
    public class GetUserOrganisationTagMatchesListQuery : IRequest<IEnumerable<UserOrganisationTagMatchListModel>>
    {
        public string UserId { get; set; }
        public string Tag { get; set; }
    }
}