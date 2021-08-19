using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Questions.Queries.GetTags
{
    public class GetQuestionTagsListQueryHandler : IRequestHandler<GetQuestionTagsListQuery, IEnumerable<QuestionTagListModel>>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public GetQuestionTagsListQueryHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<IEnumerable<QuestionTagListModel>> Handle(GetQuestionTagsListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, Constants.QuestionPlaceholder);
            if (string.IsNullOrWhiteSpace(request.Tag))
            {
                filter.AddCondition("SK", ScanOperator.BeginsWith, $"{request.QuestionId}#{Constants.TagPlaceholder}");
            }
            else
            {
                filter.AddCondition("SK", ScanOperator.BeginsWith, $"{request.QuestionId}#{Constants.TagPlaceholder}#{request.Tag}");
            }
            
            
            var response = await _dynamoDb
                .FromQueryAsync<QuestionTag>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response.Select(question => _mapper.Map(question, new QuestionTagListModel()));
        }
    }
}