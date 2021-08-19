using System.Collections.Generic;

namespace GivingAssistant.Business.Questions.Commands.Create
{
    public class CreateQuestionStatementCommandOptions
    {
        public Dictionary<string, int> TagScores { get; set; }
    }
}