using AutoMapper;
using GivingAssistant.Api.Requests.Organisations;
using GivingAssistant.Business.Organisations.Commands.Create;
using GivingAssistant.Business.Organisations.Models;

namespace GivingAssistant.Api.Mappers
{
    public class OrganisationMapper : Profile
    {
        public OrganisationMapper()
        {
            CreateMap<CreateOrganisationRequest, CreateOrganisationCommand>();
            CreateMap<OrganisationDetailModel, CreateOrganisationResponse>();
        }
    }
}