using System.Collections.Generic;
using GivingAssistant.Domain;
using NJsonSchema.Annotations;

namespace GivingAssistant.Api.Requests.Questions
{
    public class CreateQuestionRequest
    {
        [NotNull]
        public int DisplayOrder { get; set; }
        [NotNull]
        public QuestionType Type { get; set; }
        [NotNull]
        public Dictionary<string, string> Translations { get; set; }
        public List<CreateQuestionCategoryRequestOptions> CategoryOptions { get; set; }
        public CreateQuestionStatementRequestOptions StatementOptions { get; set; }
        public Dictionary<string, string> MetaTags { get; set; }
    }
}