using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetMatchesWithOrganisationsList
{
    public class GetMatchesWithOrganisationsListQueryHandler : IRequestHandler<GetMatchesWithOrganisationsListQuery, IEnumerable<UserOrganisationMatchListModel>>
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IMapper _mapper;

        public GetMatchesWithOrganisationsListQueryHandler(IAmazonDynamoDB dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserOrganisationMatchListModel>> Handle(GetMatchesWithOrganisationsListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{request.UserId}");
            filter.AddCondition("SK", ScanOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}");

            var response = await new DynamoDBContext(_dynamoDb)
                .FromQueryAsync<UserMatch>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response.Select(match => _mapper.Map(match, new UserOrganisationMatchListModel()));
        }
    }
}