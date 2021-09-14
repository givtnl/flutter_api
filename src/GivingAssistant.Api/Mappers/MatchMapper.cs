using System.Collections.Generic;
using AutoMapper;
using GivingAssistant.Api.Requests.Matches;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Business.Matches.Queries.GetUserOrganisationMatchesList;
using GivingAssistant.Business.Matches.Queries.GetUserOrganisationTagMatchesList;
using Microsoft.VisualBasic;

namespace GivingAssistant.Api.Mappers
{
    public class MatchMapper : Profile
    {
        public MatchMapper()
        {
            CreateMap<GetUserOrganisationMatchesListRequest, GetUserOrganisationMatchesListQuery>()
                .ForMember(x => x.Limit, c => c.MapFrom(d => d.Limit.GetValueOrDefault(25)))
                .ForMember(x => x.MinimumScore, c => c.MapFrom(d => d.MinimumScore.GetValueOrDefault(0)));
            CreateMap<GetUserOrganisationTagMatchesListRequest, GetUserOrganisationTagMatchesListQuery>();

            CreateMap<UserOrganisationMatchListResponse, GetUserOrganisationMatchesListResponse>()
                .ForMember(x => x.Result, c => c.MapFrom(d => d.Results))
                .ForMember(x => x.NextPageToken, c => c.MapFrom(d => d.NextPageToken));
            
            CreateMap<IEnumerable<UserOrganisationMatchListModel>, GetUserOrganisationMatchesListResponse>()
                .ForMember(x => x.Result, c => c.MapFrom(d => d));
            CreateMap<IEnumerable<UserOrganisationTagMatchListModel>, GetUserOrganisationTagMatchesListResponse>()
                .ForMember(x => x.Result, c => c.MapFrom(d => d));
        }
    }
}