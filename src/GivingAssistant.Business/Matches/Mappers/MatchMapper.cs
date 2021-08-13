using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Commands.Create;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Matches.Mappers
{
    public class MatchMapper : Profile
    {
        public MatchMapper()
        {
            CreateMap<UserMatch, UserOrganisationMatchListModel>()
                .ForMember(x => x.Organisation, c => c.MapFrom(d => d.Organisation))
                .ForPath(x => x.Organisation.Id, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(3)));
                
            CreateMap<CreateMatchCommand, IEnumerable<UserMatch>>()
                .ConvertUsing((dest, src, context) =>
                {
                    return dest.MatchingOrganisations.Select(match => new UserMatch
                    {
                        PrimaryKey = $"{Constants.UserPlaceholder}#{dest.User}",
                        SortKey = $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{match.Score}#{match.OrganisationId}#{match.Tag}",
                        Score = match.Score,
                        Tag = match.Tag,
                        Organisation = context.Mapper.Map<OrganisationDetailModel, OrganisationProfile>(match.Organisation)
                    });
                });

        }
    }
}