using System.Collections.Generic;

namespace GivingAssistant.Api.Requests.Questions
{
    public class CreateQuestionCategoryRequestOptions
    {
        public int DisplayOrder { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public Dictionary<string, int> TagScores { get; set; }
    }
}