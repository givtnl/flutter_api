using System.Collections.Generic;
using System.Linq;
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
            CreateMap<CreateAnswerCommand, List<Answer>>()
                .ConvertUsing((cmd, dst) =>
                {
                    return cmd.Answers.Select(x => new Answer
                    {
                        PrimaryKey = $"{Constants.UserPlaceholder}#{cmd.UserId}",
                        SortKey = $"{Constants.AnswerPlaceholder}#{Constants.QuestionPlaceholder}#{cmd.QuestionId}#{x.Tag}",
                        Score = x.Score
                    }).ToList();
                });
        }        
    }
}