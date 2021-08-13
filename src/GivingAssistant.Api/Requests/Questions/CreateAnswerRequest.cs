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
        public string Answer { get; set; }
    }
}