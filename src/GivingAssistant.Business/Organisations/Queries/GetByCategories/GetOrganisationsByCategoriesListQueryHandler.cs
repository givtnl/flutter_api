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

namespace GivingAssistant.Business.Organisations.Queries.GetByCategories
{
    public class GetOrganisationsByCategoriesListQueryHandler: IRequestHandler<GetOrganisationsByCategoriesListQuery, IEnumerable<OrganisationCategoryMatchListModel>>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public GetOrganisationsByCategoriesListQueryHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<OrganisationCategoryMatchListModel>> Handle(GetOrganisationsByCategoriesListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, Constants.OrganisationPlaceholder);

            if (request.Categories.Count() > 1)
            {
                // do not remove the 1 ad the end of the filter
                // this makes sure the second parameter of the between is INCLUSIVE and not EXCLUSIVE
                filter.AddCondition("SK", QueryOperator.Between, $"{Constants.MatchPlaceholder}#{Constants.CategoryPlaceholder}#{request.Categories.Min()}", $"{Constants.MatchPlaceholder}#{Constants.CategoryPlaceholder}#{request.Categories.Max()}1");
            }
            else
            {
                filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.CategoryPlaceholder}#{request.Categories.Min()}");
            }
            

            var response = await _dynamoDb
                .FromQueryAsync<OrganisationCategoryMatch>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response.Select(x => _mapper.Map(x, new OrganisationCategoryMatchListModel())).ToList();
        }
    }
}