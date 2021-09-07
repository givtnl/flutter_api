using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Questions.Queries.GetDetail
{
    public class GetQuestionDetailQueryHandler : IRequestHandler<GetQuestionDetailQuery, QuestionDetailModel>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public GetQuestionDetailQueryHandler(IDynamoDBContext dynamoDb,IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<QuestionDetailModel> Handle(GetQuestionDetailQuery request, CancellationToken cancellationToken)
        {
            var response = await _dynamoDb.LoadAsync<QuestionMetaData>(Constants.QuestionPlaceholder, $"{Constants.MetaDataPlaceholder}#{request.Id}", new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }, cancellationToken);

            return _mapper.Map(response, new QuestionDetailModel());
        }
    }
}