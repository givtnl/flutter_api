using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Organisations.Commands.Create
{
    public class CreateOrganisationCommandHandler : IRequestHandler<CreateOrganisationCommand, OrganisationDetailModel>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;

        public CreateOrganisationCommandHandler(IDynamoDBContext dynamoDb, IMapper mapper)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
        }
        public async Task<OrganisationDetailModel> Handle(CreateOrganisationCommand request, CancellationToken cancellationToken)
        {
            var convertedModel = _mapper.Map(request, new OrganisationProfile());

            var response = _mapper.Map(convertedModel, new OrganisationDetailModel());

            var writeRequest = _dynamoDb.CreateBatchWrite<OrganisationProfile>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            writeRequest.AddPutItem(convertedModel);

            var writeScoresRequest = _dynamoDb.CreateBatchWrite<OrganisationTagScore>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            writeScoresRequest.AddPutItem(_mapper.Map(request, new OrganisationTagScore(response.Id)));


            var writeMatchesRequest = _dynamoDb.CreateBatchWrite<OrganisationTagMatch>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            foreach (var requestTagScore in request.TagScores)
            {
                writeMatchesRequest.AddPutItem(_mapper.Map(requestTagScore, new OrganisationTagMatch(response.Id, convertedModel)));
            }

            await writeRequest.Combine(writeScoresRequest, writeMatchesRequest)
                .ExecuteAsync(cancellationToken);

            return response;
        }
    }
}