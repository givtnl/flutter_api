using System.Collections.Generic;
using AutoMapper;
using GivingAssistant.Api.Requests.Matches;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithOrganisationsList;

namespace GivingAssistant.Api.Mappers
{
    public class MatchMapper : Profile
    {
        public MatchMapper()
        {
            CreateMap<GetMatchesListRequest, GetMatchesWithOrganisationsListQuery>();
            CreateMap<IEnumerable<UserOrganisationMatchListModel>, GetMatchesListResponse>()
                .ForMember(x => x.Result, c => c.MapFrom(d => d));
        }
    }
}