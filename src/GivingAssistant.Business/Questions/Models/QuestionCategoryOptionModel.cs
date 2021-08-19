using System.Collections.Generic;

namespace GivingAssistant.Business.Questions.Models
{
    public class QuestionCategoryOptionModel
    {
        public int DisplayOrder { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public Dictionary<string, int> TagScores { get; set; }
    }
}