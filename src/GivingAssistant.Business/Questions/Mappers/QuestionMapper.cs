using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Questions.Mappers
{
    public class QuestionMapper : Profile
    {
        public QuestionMapper()
        {
            CreateMap<CreateQuestionCommand, Question>()
                .ForMember(x => x.PrimaryKey, x => x.MapFrom(d => nameof(Question).ToUpper()))
                .ForMember(x => x.SortKey, x => x.MapFrom(d => $"{Constants.MetaDataPlaceholder}#{Guid.NewGuid()}"));

            CreateMap<Question, QuestionDetailModel>()
                .ForMember(x => x.Id, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.None).ElementAtOrDefault(1)));

            CreateMap<Question, QuestionListModel>()
                .ForMember(x => x.Id, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.None).ElementAtOrDefault(1)));

            CreateMap<KeyValuePair<string, int>, QuestionTag>()
                .ForMember(x => x.Score, c => c.MapFrom(d => d.Value))
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.Key))
                .ForMember(x => x.PrimaryKey, x => x.MapFrom(d => nameof(Question).ToUpper()))
                .ForMember(x => x.SortKey, x => x.MapFrom((dest, src) => $"{src.QuestionId}#{Constants.TagPlaceholder}#{dest.Key}"));


        }
    }
}