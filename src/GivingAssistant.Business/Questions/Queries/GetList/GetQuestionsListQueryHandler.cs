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
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Questions.Queries.GetList
{
    public class GetQuestionsListQueryHandler : IRequestHandler<GetQuestionsListQuery, IEnumerable<QuestionListModel>>
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IMapper _mapper;

        public GetQuestionsListQueryHandler(IAmazonDynamoDB dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<IEnumerable<QuestionListModel>> Handle(GetQuestionsListQuery request, CancellationToken cancellationToken)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, nameof(Question).ToUpper());

            filter.AddCondition("SK", QueryOperator.BeginsWith, Constants.MetaDataPlaceholder);

            var response = await new DynamoDBContext(_dynamoDb)
                .FromQueryAsync<Question>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync(cancellationToken);

            return response.Select(question => _mapper.Map(question, new QuestionListModel()));
        }
    }
}