using System;
using System.Linq;
using AutoMapper;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Matches.Mappers
{
    public class MatchMapper : Profile
    {
        public MatchMapper()
        {
            CreateMap<UserMatch, UserOrganisationMatchListModel>()
                .ForMember(x => x.Organisation,
                    c => c.MapFrom((src, dest) =>
                        src.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3)));

        }
    }
}