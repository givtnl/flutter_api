using MediatR;

namespace GivingAssistant.Business.Answers.Commands.Create
{
    public class CreateAnswerCommand : IRequest
    {
        public string UserId { get; set; }
        public string QuestionId { get; set; }
        public string Answer { get; set; }
    }
}