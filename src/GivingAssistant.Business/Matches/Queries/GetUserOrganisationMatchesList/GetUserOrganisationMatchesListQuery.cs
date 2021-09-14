using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetUserOrganisationMatchesList
{
    public class GetUserOrganisationMatchesListQuery : IRequest<UserOrganisationMatchListResponse>
    {
        public string UserId { get; set; }
        public string NextPageToken { get; set; }
        public int MinimumScore { get; set; }
        public int Limit { get; set; }
    }
}