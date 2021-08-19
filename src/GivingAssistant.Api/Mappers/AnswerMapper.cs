using AutoMapper;
using GivingAssistant.Api.Requests.Questions;
using GivingAssistant.Business.Answers.Commands.Create;
using MediatR;

namespace GivingAssistant.Api.Mappers
{
    public class AnswerMapper : Profile
    {
        public AnswerMapper()
        {
            CreateMap<CreateAnswerRequest, CreateAnswerCommand>();
            CreateMap<CreateAnswerDetailRequest, CreateAnswerDetailCommand>();
            CreateMap<Unit, CreateAnswerResponse>(MemberList.None);
        }
    }
}