using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Questions.Commands.Create
{
    public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, QuestionDetailModel>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public CreateQuestionCommandHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<QuestionDetailModel> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
        {
            var convertedModel = _mapper.Map(request, new Question());

            var response = _mapper.Map(convertedModel, new QuestionDetailModel());

            var writeRequest = _dynamoDb.CreateBatchWrite<Question>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            writeRequest.AddPutItem(convertedModel);

            var tagWriteRequest = _dynamoDb.CreateBatchWrite<QuestionTag>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            foreach (var requestTagScore in request.TagScores)
            {
                tagWriteRequest.AddPutItem(_mapper.Map(requestTagScore, new QuestionTag(response.Id)));
            }

            await writeRequest.Combine(tagWriteRequest).ExecuteAsync(cancellationToken);

            return response;
        }
    }
}