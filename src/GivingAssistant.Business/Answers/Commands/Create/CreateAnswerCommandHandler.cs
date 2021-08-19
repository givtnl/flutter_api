using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Answers.Commands.Create
{
    public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, Unit>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public CreateAnswerCommandHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
        {
            var convertedModel = _mapper.Map(request, new List<Answer>());

            var batchWrite = _dynamoDb.CreateBatchWrite<Answer>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            batchWrite.AddPutItems(convertedModel);

            await batchWrite.ExecuteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}