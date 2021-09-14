using System.Collections.Generic;
using GivingAssistant.Domain;

namespace GivingAssistant.Business.Questions.Models
{
    public class QuestionListModel
    {
        public string Id { get; set; }
        public int DisplayOrder { get; set; }
        public QuestionType Type { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public List<QuestionCategoryOptionModel> CategoryOptions { get; set; }
        public QuestionStatementModel StatementOptions { get; set; }
        public Dictionary<string, string> MetaTags { get; set; }
    }
}