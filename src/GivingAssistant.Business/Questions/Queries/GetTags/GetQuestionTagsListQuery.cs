using System.Collections.Generic;
using GivingAssistant.Business.Questions.Models;
using MediatR;

namespace GivingAssistant.Business.Questions.Queries.GetTags
{
    public class GetQuestionTagsListQuery : IRequest<IEnumerable<QuestionTagListModel>>
    {
        public string QuestionId { get; set; }
    }
}