using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch
{
    public class CreateUserOrganisationMatchCommandHandler : IRequestHandler<CreateUserOrganisationMatchCommand, Unit>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public CreateUserOrganisationMatchCommandHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(CreateUserOrganisationMatchCommand request, CancellationToken cancellationToken)
        {
            var writeRequest = _dynamoDb.CreateBatchWrite<UserOrganisationMatch>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            writeRequest.AddPutItems(_mapper.Map<CreateUserOrganisationMatchCommand, IEnumerable<UserOrganisationMatch>>(request));

            await writeRequest.ExecuteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
