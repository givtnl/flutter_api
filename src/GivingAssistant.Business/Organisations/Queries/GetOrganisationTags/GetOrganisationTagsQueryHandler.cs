using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Organisations.Queries.GetOrganisationTags
{
    public class GetOrganisationTagsQueryHandler : IRequestHandler<GetOrganisationTagsQuery, IEnumerable<OrganisationTagMatchListModel>>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public GetOrganisationTagsQueryHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<IEnumerable<OrganisationTagMatchListModel>> Handle(GetOrganisationTagsQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, Constants.OrganisationPlaceholder);
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}");
    
            var response = await _dynamoDb
                .FromQueryAsync<OrganisationTagMatch>(new QueryOperationConfig
                {
                    
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response
                .Where(tagMatch => tagMatch.SortKey.EndsWith(request.OrganisationId))
                .Select(x => _mapper.Map(x, new OrganisationTagMatchListModel()));
        }
    }
}