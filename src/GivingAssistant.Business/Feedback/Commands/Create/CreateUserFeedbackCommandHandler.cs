using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using MediatR;
using Constants = GivingAssistant.Business.Infrastructure.Constants;

namespace GivingAssistant.Business.Feedback
{
    public class CreateUserFeedbackCommandHandler: IRequestHandler<CreateUserFeedbackCommand>
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
            var convertedModel = _mapper.Map(request, new Persistence.UserFeedback());

            var writeRequest = _dynamoDb.CreateBatchWrite<Persistence.UserFeedback>(new DynamoDBOperationConfig()
            {
                OverrideTableName = Constants.TableName
            });
            
            writeRequest.AddPutItem(convertedModel);

            await writeRequest.ExecuteAsync(cancellationToken);
            
            return Unit.Value;
        }
    }
}