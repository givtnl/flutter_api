using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.DeleteUserOrganisationMatch
{
    public class DeleteAllUserOrganisationMatchesCommandHandler: IRequestHandler<DeleteAllUserOrganisationMatchesCommand, Unit>
    {
        private readonly IDynamoDBContext _context;

        public DeleteAllUserOrganisationMatchesCommandHandler(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteAllUserOrganisationMatchesCommand request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{request.UserId}");
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}");

            var response = await _context
                .FromQueryAsync<UserTagMatch>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            foreach (var match in response)
            {
                await _context.DeleteAsync(match, CancellationToken.None);
            }
            
            return Unit.Value;
        }
    }
}