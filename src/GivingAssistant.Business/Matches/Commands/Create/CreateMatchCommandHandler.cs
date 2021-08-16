using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.Create
{
    public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, Unit>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public CreateMatchCommandHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
        {
            var writeRequest = _dynamoDb.CreateBatchWrite<UserMatch>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            writeRequest.AddPutItems(_mapper.Map<CreateMatchCommand, IEnumerable<UserMatch>>(request));

            await writeRequest.ExecuteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
