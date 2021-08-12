using System.Collections.Generic;
using GivingAssistant.Business.Questions.Models;
using NJsonSchema.Annotations;

namespace GivingAssistant.Api.Requests.Questions
{
    public class GetQuestionsListResponse
    {
        [NotNull]
        public IEnumerable<QuestionListModel> Result { get; set; }
    }
}