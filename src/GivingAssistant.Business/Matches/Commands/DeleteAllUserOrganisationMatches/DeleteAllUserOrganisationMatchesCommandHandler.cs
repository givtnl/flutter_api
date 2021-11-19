using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.DeleteUserOrganisationMatch
{
    public class DeleteAllUserOrganisationMatchesCommandHandler: IRequestHandler<DeleteAllUserOrganisationMatchesCommand, Unit>
    {
        private readonly IDynamoDBContext _context;
        private readonly IAmazonDynamoDB _client;

        public DeleteAllUserOrganisationMatchesCommandHandler(IDynamoDBContext context, IAmazonDynamoDB client)
        {
            _context = context;
            _client = client;
        }

        public async Task<Unit> Handle(DeleteAllUserOrganisationMatchesCommand request, CancellationToken cancellationToken)
        {
            var query = await _client.QueryAsync(new QueryRequest(Constants.TableName)
            {
                ScanIndexForward = false,
                Select = Select.ALL_ATTRIBUTES,
                KeyConditions = new Dictionary<string, Condition>
                {
                    {
                        "PK", new Condition
                        {
                            ComparisonOperator = ComparisonOperator.EQ, AttributeValueList = new List<AttributeValue>
                            {
                                new($"{Constants.UserPlaceholder}#{request.UserId}")
                            }
                        }
                    },
                    {
                        "SK", new Condition
                        {
                            ComparisonOperator = ComparisonOperator.BEGINS_WITH,
                            AttributeValueList = new List<AttributeValue>
                            {
                                new($"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TotalScorePlaceHolder}")
                            }
                        }
                    }
                }
            }, cancellationToken);

            var itemsToDelete = query.Items.Select(x => _context.FromDocument<UserOrganisationMatch>(Document.FromAttributeMap(x)));

            foreach (var match in itemsToDelete)
            {
                await _context.DeleteAsync(match, CancellationToken.None);
            }
            
            return Unit.Value;
        }
    }
}