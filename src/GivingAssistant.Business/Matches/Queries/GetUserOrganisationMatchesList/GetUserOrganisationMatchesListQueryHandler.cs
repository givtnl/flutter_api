using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetUserOrganisationMatchesList
{
    public class GetUserOrganisationMatchesListQueryHandler : IRequestHandler<GetUserOrganisationMatchesListQuery, UserOrganisationMatchListResponse>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;
        private readonly IAmazonDynamoDB _client;

        public GetUserOrganisationMatchesListQueryHandler(IDynamoDBContext dynamoDb, IMapper mapper, IAmazonDynamoDB client)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
            _client = client;
        }

        public async Task<UserOrganisationMatchListResponse> Handle(GetUserOrganisationMatchesListQuery request, CancellationToken cancellationToken)
        {
            var query = await _client.QueryAsync(new QueryRequest(Constants.TableName)
            {
                Limit = request.Limit,
                ScanIndexForward = false,
                Select = Select.ALL_ATTRIBUTES,
                ExclusiveStartKey = 
                    string.IsNullOrWhiteSpace(request.NextPageToken) ?  null :
                    new Dictionary<string, AttributeValue>
                    {
                        {"PK", new AttributeValue($"{Constants.UserPlaceholder}#{request.UserId}")},
                        {"SK", new AttributeValue(request.NextPageToken)}
                    },
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
                            ComparisonOperator = ComparisonOperator.GE,
                            AttributeValueList = new List<AttributeValue>
                            {
                                new($"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TotalScorePlaceHolder}#{request.MinimumScore}")
                            }
                        }
                    }
                }
            }, cancellationToken);
            
            var pagedSet = query.Items.Select(x => _dynamoDb.FromDocument<UserOrganisationMatch>(Document.FromAttributeMap(x)));

            return new UserOrganisationMatchListResponse(
                pagedSet.Select(match => _mapper.Map(match, new UserOrganisationMatchListModel())).ToList(),
                query.LastEvaluatedKey != null && query.LastEvaluatedKey.Any() ? query.LastEvaluatedKey["SK"]?.S : null
            );
        }
    }
}