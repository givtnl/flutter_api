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
    public class GetOrganisationsByTagsListQuery : IRequest<IEnumerable<OrganisationTagMatchListModel>>
    {
        public IEnumerable<string> Tags { get; set; }
    }

    public class GetOrganisationsByTagsListQueryHandler : IRequestHandler<GetOrganisationsByTagsListQuery, IEnumerable<OrganisationTagMatchListModel>>
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IMapper _mapper;

        public GetOrganisationsByTagsListQueryHandler(IAmazonDynamoDB dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrganisationTagMatchListModel>> Handle(GetOrganisationsByTagsListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, Constants.OrganisationPlaceholder);
            filter.AddCondition("SK", ScanOperator.GreaterThanOrEqual, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{request.Tags.Min()}");
            filter.AddCondition("SK", ScanOperator.LessThanOrEqual, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{request.Tags.Max()}");

            var response = await new DynamoDBContext(_dynamoDb)
                .FromQueryAsync<OrganisationTagMatch>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response.Select(x => _mapper.Map(x, new OrganisationTagMatchListModel()));
        }
    }

}
