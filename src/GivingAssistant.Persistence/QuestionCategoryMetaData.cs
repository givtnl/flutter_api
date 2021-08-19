using System.Collections.Generic;

namespace GivingAssistant.Persistence
{
    public class QuestionCategoryMetaData
    {
        public int DisplayOrder { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public Dictionary<string, int> TagScores { get; set; }
    }
}