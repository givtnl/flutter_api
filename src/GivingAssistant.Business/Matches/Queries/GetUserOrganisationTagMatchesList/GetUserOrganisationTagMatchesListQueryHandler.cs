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

namespace GivingAssistant.Business.Matches.Queries.GetUserOrganisationTagMatchesList
{
    public class GetUserOrganisationTagMatchesListQueryHandler : IRequestHandler<GetUserOrganisationTagMatchesListQuery, IEnumerable<UserOrganisationTagMatchListModel>>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public GetUserOrganisationTagMatchesListQueryHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<UserOrganisationTagMatchListModel>> Handle(GetUserOrganisationTagMatchesListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{request.UserId}");
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TagPlaceholder}#{request.OrganisationId}#");

            var response = await _dynamoDb
                .FromQueryAsync<UserOrganisationTagMatch>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response.Select(match => _mapper.Map(match, new UserOrganisationTagMatchListModel())).ToList();
        }
    }
}