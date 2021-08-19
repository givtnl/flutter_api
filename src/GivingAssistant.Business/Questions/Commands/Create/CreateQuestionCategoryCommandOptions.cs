using System.Collections.Generic;

namespace GivingAssistant.Business.Questions.Commands.Create
{
    public class CreateQuestionCategoryCommandOptions
    {
        public int DisplayOrder { get; set; }
        public Dictionary<string,string> Translations { get; set; }
        public Dictionary<string, int> TagScores { get; set; }
    }
}