using System.Collections.Generic;

namespace GivingAssistant.Api.Requests.Questions
{
    public class CreateQuestionStatementRequestOptions
    {
        public Dictionary<string, int> TagScores { get; set; }
    }
}