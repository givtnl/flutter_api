using MediatR;

namespace GivingAssistant.Business.Matches.Commands.DeleteUserOrganisationMatch
{
    public class DeleteAllUserOrganisationMatchesCommand: IRequest
    {
        public string UserId { get; set; }
    }
}