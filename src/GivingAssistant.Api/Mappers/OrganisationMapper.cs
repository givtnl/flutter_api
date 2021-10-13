using System.Collections.Generic;
using AutoMapper;
using GivingAssistant.Api.Requests.Organisations;
using GivingAssistant.Business.Organisations.Commands.Create;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Business.Organisations.Queries.GetOrganisationTags;

namespace GivingAssistant.Api.Mappers
{
    public class OrganisationMapper : Profile
    {
        public OrganisationMapper()
        {
            CreateMap<CreateOrganisationRequest, CreateOrganisationCommand>();
            CreateMap<OrganisationDetailModel, CreateOrganisationResponse>();
            CreateMap<GetOrganisationTagsRequest, GetOrganisationTagsQuery>();
            CreateMap<IEnumerable<OrganisationTagMatchListModel>, GetOrganisationTagsResponse>()
                .ForMember(x => x.Results, c => c.MapFrom(d => d));
        }
    }
}