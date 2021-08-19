using System.Collections.Generic;
using MediatR;

namespace GivingAssistant.Business.Answers.Commands.Create
{
    public class CreateAnswerCommand : IRequest
    {
        public string UserId { get; set; }
        public string QuestionId { get; set; }
        public List<CreateAnswerDetailCommand> Answers { get; set; } = new();
    }

    public class CreateAnswerDetailCommand
    {
        public string Tag { get; set; }
        public decimal Score { get; set; }
    }
}