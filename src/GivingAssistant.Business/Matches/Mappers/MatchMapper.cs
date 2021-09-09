using System;
using System.Linq;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Commands.CreateUserTagMatch;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Matches.Mappers
{
    public class MatchMapper : Profile
    {
        public MatchMapper()
        {
            CreateMap<CreateUserTagMatchCommand, UserTagMatch>()
                .ForMember(x => x.PrimaryKey, c => c.MapFrom(d => $"{Constants.UserPlaceholder}#{d.User}"))
                .ForMember(x => x.SortKey, c => c.MapFrom(d => $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{d.Question.Tag}"))
                .ForMember(x => x.MaximumScore, c => c.MapFrom((d, src) => src.MaximumScore + d.Question.Score))
                .ForMember(x => x.CurrentScore, c => c.MapFrom((d, src) => src.CurrentScore + ((Convert.ToDecimal(d.Question.Score) * d.Answer))))
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.Question.Tag))
                .AfterMap((cmd, tag, context) => tag.Percentage = (int) (Convert.ToDecimal(tag.CurrentScore) / Convert.ToDecimal(tag.MaximumScore) * 100));

            CreateMap<UserOrganisationMatch, UserOrganisationMatchListModel>()
                .ForMember(x => x.Organisation, c => c.MapFrom(d => d.Organisation));
            // .ForPath(x => x.Organisation.Id, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3)));

            CreateMap<UserOrganisationTagMatch, UserOrganisationTagMatchListModel>()
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(4)))
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3)));

            CreateMap<UserTagMatch, UserTagMatchListModel>()
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(2)))
                .ForMember(x => x.User, c => c.MapFrom(d => d.PrimaryKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1)));
        }
    }
}