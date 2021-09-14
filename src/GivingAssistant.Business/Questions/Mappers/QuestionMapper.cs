using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Domain;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Questions.Mappers
{
    public class QuestionMapper : Profile
    {
        public QuestionMapper()
        {
            CreateMap<CreateQuestionCommand, QuestionMetaData>()
                .ForMember(x => x.PrimaryKey, x => x.MapFrom(d => Constants.QuestionPlaceholder))
                .ForMember(x => x.SortKey, x => x.MapFrom(d => $"{Constants.MetaDataPlaceholder}#{Guid.NewGuid()}"))
                .ForMember(x => x.CategoryOptions, c => c.MapFrom(d => d.Type == QuestionType.Category ? d.CategoryOptions : null))
                .ForMember(x => x.StatementOptions, c => c.MapFrom(d => d.Type == QuestionType.Statement ? d.StatementOptions : null))
                .ForMember(x => x.MetaTags, c => c.MapFrom(d => d.MetaTags.Select(QuestionMetaTag.FromKeyValuePair)));


            CreateMap<QuestionMetaData, QuestionDetailModel>()
                .ForMember(x => x.Id, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.None).ElementAtOrDefault(1)));

            CreateMap<QuestionMetaData, QuestionListModel>()
                .ForMember(x => x.Id, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.None).ElementAtOrDefault(1)))
                .ForMember(x => x.MetaTags, c => c.MapFrom(d => d.MetaTags.Select(x => x.ToKeyValuePair())));

            //AnimalsQuestion#TAG#animals#SCORE#80
            CreateMap<QuestionTag, QuestionTagListModel>()
                .ForMember(x => x.QuestionId, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(0)))
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.SortKey.Split('#', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(2)));

            CreateMap<CreateQuestionStatementCommandOptions, QuestionStatementMetaData>(MemberList.Destination);
            CreateMap<CreateQuestionCategoryCommandOptions, QuestionCategoryMetaData>(MemberList.Destination);
            CreateMap<QuestionStatementMetaData, QuestionStatementModel>(MemberList.Destination);
            CreateMap<QuestionCategoryMetaData, QuestionCategoryOptionModel>(MemberList.Destination);

            CreateMap<KeyValuePair<string, int>, QuestionTag>()
                .ForMember(x => x.Score, c => c.MapFrom(d => d.Value))
                .ForMember(x => x.Tag, c => c.MapFrom(d => d.Key))
                .ForMember(x => x.PrimaryKey, x => x.MapFrom(d => Constants.QuestionPlaceholder))
                .ForMember(x => x.SortKey, x => x.MapFrom((dest, src) => $"{src.QuestionId}#{Constants.TagPlaceholder}#{dest.Key}"));


        }
    }
}