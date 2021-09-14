using System.Collections.Generic;
using GivingAssistant.Domain;

namespace GivingAssistant.Persistence
{
    
    public class QuestionMetaData : BaseItem
    {
        public int DisplayOrder { get; set; }
        public QuestionType Type { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public QuestionStatementMetaData StatementOptions { get; set; }
        public List<QuestionCategoryMetaData> CategoryOptions { get; set; }
        public List<QuestionMetaTag> MetaTags { get; set; }
    }
}