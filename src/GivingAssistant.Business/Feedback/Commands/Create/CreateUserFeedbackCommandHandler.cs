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
            var existingFeedback = await _dynamoDb.LoadAsync<Persistence.UserFeedback>($"{Constants.FeedbackPlaceholder}",$"{Constants.UserPlaceholder}#{request.UserId}", new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }, cancellationToken) ?? new Persistence.UserFeedback();
            
            await _dynamoDb.SaveAsync(_mapper.Map(request, existingFeedback), new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }, cancellationToken);
            return Unit.Value;
        }
    }
}