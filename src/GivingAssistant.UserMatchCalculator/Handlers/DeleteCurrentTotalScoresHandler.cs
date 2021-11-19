using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GivingAssistant.UserMatchCalculator.Models;
using GivingAssistant.Business.Matches.Commands.DeleteUserOrganisationMatch;

namespace GivingAssistant.UserMatchCalculator.Handlers
{
    public class DeleteCurrentTotalScoresHandler : IAnsweredQuestionHandler
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonDynamoDB _client;
        public int ExecutionOrder => int.MaxValue - 1;

        public DeleteCurrentTotalScoresHandler(IDynamoDBContext context, IAmazonDynamoDB client)
        {
            _context = context;
            _client = client;
        }

        public async Task Handle(HandleAnsweredQuestionRequest request)
        {
            await new DeleteAllUserOrganisationMatchesCommandHandler(_context).Handle(new DeleteAllUserOrganisationMatchesCommand
            {
                UserId = request.User
            }, CancellationToken.None);
        }

        public bool CanHandle(HandleAnsweredQuestionRequest request)
        {
            return true;
        }
    }
}