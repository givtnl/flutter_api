using GivingAssistant.Business.Questions.Models;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserTagMatch
{
    public class CreateUserTagMatchCommand : IRequest
    {
        public string User { get; set; }
        public QuestionTagListModel Question { get; set; }
        public int Answer { get; set; }
    }
}