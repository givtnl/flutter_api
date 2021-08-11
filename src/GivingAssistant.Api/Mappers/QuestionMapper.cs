using System.Collections.Generic;
using AutoMapper;
using GivingAssistant.Api.Requests.Questions;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Business.Questions.Queries.GetList;

namespace GivingAssistant.Api.Mappers
{
    public class QuestionMapper : Profile
    {
        public QuestionMapper()
        {
            CreateMap<GetQuestionsListRequest, GetQuestionsListQuery>();

            CreateMap<IEnumerable<QuestionListModel>, GetQuestionsListResponse>()
                .ForMember(x => x.Result, c => c.MapFrom(d => d));

            CreateMap<CreateQuestionRequest, CreateQuestionCommand>();

            CreateMap<QuestionDetailModel, CreateQuestionResponse>();
        }
    }
}