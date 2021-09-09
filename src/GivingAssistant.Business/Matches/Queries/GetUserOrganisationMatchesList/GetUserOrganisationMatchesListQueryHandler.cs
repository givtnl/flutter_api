using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Queries.GetUserOrganisationMatchesList
{
    public class GetUserOrganisationMatchesListQueryHandler : IRequestHandler<GetUserOrganisationMatchesListQuery, IEnumerable<UserOrganisationMatchListModel>>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public GetUserOrganisationMatchesListQueryHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserOrganisationMatchListModel>> Handle(GetUserOrganisationMatchesListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{request.UserId}");

            filter.AddCondition("SK", QueryOperator.GreaterThanOrEqual,
                $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TotalScorePlaceHolder}#{request.MinimumScore}#");
            filter.AddCondition("SK", QueryOperator.LessThanOrEqual, $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TotalScorePlaceHolder}#100#");

            var response = await _dynamoDb
                .FromQueryAsync<UserOrganisationMatch>(new QueryOperationConfig
                {
                    Filter = filter,
                    BackwardSearch = true,
                    Limit = request.Limit
                }, new DynamoDBOperationConfig {OverrideTableName = Constants.TableName}).GetRemainingAsync(cancellationToken);

            return response.Select(match => _mapper.Map(match, new UserOrganisationMatchListModel()));
        }
    }
}