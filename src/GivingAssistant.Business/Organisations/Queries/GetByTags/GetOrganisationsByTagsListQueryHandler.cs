using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Organisations.Queries.GetByTags
{
    public class GetOrganisationsByTagsListQueryHandler : IRequestHandler<GetOrganisationsByTagsListQuery, IEnumerable<OrganisationTagMatchListModel>>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public GetOrganisationsByTagsListQueryHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrganisationTagMatchListModel>> Handle(GetOrganisationsByTagsListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, Constants.OrganisationPlaceholder);

            if (request.Tags.Count() > 1)
            {
                filter.AddCondition("SK", QueryOperator.Between, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{request.Tags.Min()}", $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{request.Tags.Max()}");
            }
            else
            {
                filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{request.Tags.Min()}");
            }
            

            var response = await _dynamoDb
                .FromQueryAsync<OrganisationTagMatch>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response.Select(x => _mapper.Map(x, new OrganisationTagMatchListModel()));
        }
    }
}