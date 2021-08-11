using System.Collections.Generic;
using GivingAssistant.Domain;

namespace GivingAssistant.Persistence
{
    
    public class Question : BaseItem
    {
        public int DisplayOrder { get; set; }
        public QuestionType Type { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public Dictionary<string, decimal> TagScores { get; set; }
    }
}