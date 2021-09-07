using System.Collections.Generic;
using AutoMapper;
using GivingAssistant.Api.Requests.Questions;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Business.Questions.Queries.GetList;
using GivingAssistant.Domain;

namespace GivingAssistant.Api.Mappers
{
    public class QuestionMapper : Profile
    {
        public QuestionMapper()
        {
            CreateMap<GetQuestionsListRequest, GetQuestionsListQuery>();

            CreateMap<IEnumerable<QuestionListModel>, GetQuestionsListResponse>()
                .ForMember(x => x.Result, c => c.MapFrom(d => d));

            CreateMap<CreateQuestionRequest, CreateQuestionCommand>()
                .ForMember(x => x.CategoryOptions, c => c.MapFrom(d => d.Type == QuestionType.Category ? d.CategoryOptions : new List<CreateQuestionCategoryRequestOptions>()))
                .ForMember(x => x.StatementOptions, c => c.MapFrom(d => d.Type == QuestionType.Statement ? d.StatementOptions : null));

            CreateMap<CreateQuestionStatementRequestOptions, CreateQuestionStatementCommandOptions>();
            CreateMap<CreateQuestionCategoryRequestOptions, CreateQuestionCategoryCommandOptions>();

            CreateMap<QuestionDetailModel, CreateQuestionResponse>();
        }
    }
}