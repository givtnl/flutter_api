using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Organisations.Commands.Create;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Organisations.Mappers
{
    public class OrganisationsMapper : Profile
    {
        public OrganisationsMapper()
        {
            CreateMap<CreateOrganisationCommand, OrganisationProfile>()
                .ForMember(x => x.MetaTags, x => x.MapFrom(d => d.MetaTags.Select(OrganisationMetaTag.FromKeyValuePair)))
                .ForMember(x => x.PrimaryKey, x => x.MapFrom((dest,src) => $"{Constants.OrganisationPlaceholder}#{src.Id}"))
                .ForMember(x => x.SortKey, x => x.MapFrom(d => $"{Constants.MetaDataPlaceholder}#{Constants.ProfilePlaceholder}"));

            CreateMap<OrganisationProfile, OrganisationDetailModel>()
                .ForMember(x => x.MetaTags, c => c.MapFrom(d => new Dictionary<string,string>(d.MetaTags.Select(x => x.ToKeyValuePair()))))
                .ForMember(x => x.Id, c => c.MapFrom(d => d.PrimaryKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1)));

            CreateMap<OrganisationDetailModel, OrganisationProfile>();



            CreateMap<CreateOrganisationCommand, OrganisationTagScore>()
                .ForMember(x => x.PrimaryKey, c => c.MapFrom((src,dest) => $"{Constants.OrganisationPlaceholder}#{dest.OrganisationId}"))
                .ForMember(x => x.SortKey, c => c.MapFrom((src, dest) => $"{Constants.MetaDataPlaceholder}#{Constants.ScorePlaceholder}"))
                .ForMember(x => x.Scores,c => c.MapFrom(d => d.TagScores.Select(OrganisationTagScoreDetail.FromDictionaryEntry).ToList()));

            CreateMap<KeyValuePair<string,int>, OrganisationTagMatch>()
                .ForMember(x => x.Score, c => c.MapFrom(d => d.Value))
                .ForMember(x => x.Tag, c=> c.MapFrom(d => d.Key))
                .ForMember(x => x.PrimaryKey, c => c.MapFrom(d => Constants.OrganisationPlaceholder))
                .ForMember(x => x.SortKey, c => c.MapFrom((d,s) => $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{d.Key}#{s.OrganisationId}"));

            CreateMap<OrganisationTagMatch, OrganisationTagMatchListModel>()
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(2)))
                .ForMember(x => x.OrganisationId, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3)));
        }
    }
}