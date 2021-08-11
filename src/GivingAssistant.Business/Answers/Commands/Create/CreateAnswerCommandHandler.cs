using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Answers.Commands.Create
{
    public class CreateAnswerCommandHandler : IRequestHandler<CreateAnswerCommand, Unit>
    {
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IMapper _mapper;

        public CreateAnswerCommandHandler(IAmazonDynamoDB dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(CreateAnswerCommand request, CancellationToken cancellationToken)
        {
            var convertedModel = _mapper.Map(request, new Answer());

            await new DynamoDBContext(_dynamoDb).SaveAsync(convertedModel, new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }, cancellationToken);

            return Unit.Value;
        }
    }
}