using AutoMapper;
using GivingAssistant.Business.Answers.Commands.Create;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Answers.Mappers
{
    public class AnswerMapper : Profile
    {
        public AnswerMapper()
        {
            CreateMap<CreateAnswerCommand, Answer>()
                .ForMember(x => x.PrimaryKey, c => c.MapFrom(d => $"{Constants.UserPlaceholder}#{d.UserId}"))
                .ForMember(x => x.SortKey, c => c.MapFrom(d => $"{Constants.AnswerPlaceholder}#{Constants.QuestionPlaceholder}#{d.QuestionId}"));
        }        
    }
}