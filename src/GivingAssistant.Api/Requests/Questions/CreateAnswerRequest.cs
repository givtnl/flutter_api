using System.Collections.Generic;
using NJsonSchema.Annotations;
using NSwag.Annotations;

namespace GivingAssistant.Api.Requests.Questions
{
    public class CreateAnswerRequest
    {
        [NotNull]
        public string UserId { get; set; }
        [NotNull]
        [OpenApiIgnore]
        public string QuestionId { get; set; }
        [NotNull] 
        public List<CreateAnswerDetailRequest> Answers { get; set; } = new();
    }

    public class CreateAnswerDetailRequest
    {
        [NotNull]
        public string Tag { get; set; }
        [NotNull]
        public decimal Score { get; set; }
    }
}