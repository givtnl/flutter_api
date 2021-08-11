using System.Collections.Generic;
using GivingAssistant.Business.Questions.Models;

namespace GivingAssistant.Api.Requests.Questions
{
    public class GetQuestionsListResponse
    {
        public IEnumerable<QuestionListModel> Result { get; set; }
    }
}