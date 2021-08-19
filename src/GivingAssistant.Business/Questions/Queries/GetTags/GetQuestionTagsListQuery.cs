using System.Collections.Generic;
using GivingAssistant.Business.Questions.Models;
using MediatR;

namespace GivingAssistant.Business.Questions.Queries.GetTags
{
    public class GetQuestionTagsListQuery : IRequest<IEnumerable<QuestionTagListModel>>
    {
        public GetQuestionTagsListQuery(string questionId)
        {
            QuestionId = questionId;
        }
        public GetQuestionTagsListQuery()
        {
            
        }
        public string QuestionId { get; set; }
        public string Tag { get; set; }
    }
}