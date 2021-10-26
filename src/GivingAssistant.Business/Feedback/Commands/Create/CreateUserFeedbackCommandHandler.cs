using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using MediatR;

namespace GivingAssistant.Business.Feedback
{
    public class CreateUserFeedbackCommandHandler : IRequestHandler<CreateUserFeedbackCommand>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public CreateUserFeedbackCommandHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateUserFeedbackCommand request, CancellationToken cancellationToken)
        {
            await _dynamoDb.SaveAsync(_mapper.Map(request, new Persistence.UserFeedback()), new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }, cancellationToken);
            return Unit.Value;
        }
    }
}