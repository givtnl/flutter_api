using System.Collections.Generic;
using GivingAssistant.Business.Matches.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetMatchesWithTagsList
{
    public class GetMatchesWithTagsListQuery : IRequest<IEnumerable<UserTagMatchListModel>>
    {
        public string UserId { get; set; }
    }
}