using System.Collections.Generic;
using Amazon.Lambda.Core;
using GivingAssistant.Business.Questions.Models;

namespace GivingAssistant.UserMatchCalculator.Models
{
    public class HandleAnsweredQuestionRequest
    {
        public string AnsweredTag { get; set; }
        public IEnumerable<QuestionTagListModel> QuestionTags { get; set; }
        // public IEnumerable<UserTagMatchListModel> UserTags { get; set; }
        public QuestionDetailModel AnsweredQuestion { get; set; }
        public string User { get; set; }
        public decimal Answer { get; set; }
        public ILambdaContext LambdaContext { get; set; }
    }
}