using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserCategoryMatch
{
    public class CreateUserCategoryMatchCommand: IRequest
    {
        public string User { get; set; }
        public string Category { get; set; }
    }
}