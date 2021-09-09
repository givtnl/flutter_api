using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetMatchesWithOrganisationsList
{
    public class GetMatchesWithOrganisationsListQuery : IRequest<IEnumerable<UserOrganisationMatchListModel>>
    {
        public string UserId { get; set; }
        public int? MinimumScore { get; set; }
    }
}