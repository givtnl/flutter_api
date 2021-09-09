using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetUserTagMatchesList
{
    public class GetUserTagMatchesListQuery : IRequest<IEnumerable<UserTagMatchListModel>>
    {
        public string UserId { get; set; }
        public List<string> Tags { get; set; }
    }
}