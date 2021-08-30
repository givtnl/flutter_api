using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch
{
    public class CreateUserOrganisationMatchCommandHandler : IRequestHandler<CreateUserOrganisationMatchCommand, Unit>
    {
        private readonly IDynamoDBContext _dynamoDb;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IUserOrganisationMatcher> _matchMakers;

        public CreateUserOrganisationMatchCommandHandler(IDynamoDBContext dynamoDb, IMapper mapper, IEnumerable<IUserOrganisationMatcher> matchMakers)
        {
            _dynamoDb = dynamoDb;
            _mapper = mapper;
            _matchMakers = matchMakers;
        }
        public async Task<Unit> Handle(CreateUserOrganisationMatchCommand request, CancellationToken cancellationToken)
        {
            var evaluatedScores = await CalculateScoresForOrganisations(request, cancellationToken);

            var writeRequest = _dynamoDb.CreateBatchWrite<UserOrganisationMatch>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });

            foreach (var keyValuePair in evaluatedScores)
            {
                var organisation = request.MatchingOrganisations
                    .Where(x => x.OrganisationId == keyValuePair.Key)
                    .Select(x => _mapper.Map(x.Organisation, new OrganisationProfile()))
                    .FirstOrDefault();
                
                if (organisation == null)
                    continue;
                
                writeRequest.AddPutItem(new UserOrganisationMatch
                {
                    Organisation = organisation,
                    Score = keyValuePair.Value,
                    PrimaryKey = $"{Constants.UserPlaceholder}#{request.User}",
                    SortKey = $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{keyValuePair.Key}"
                });
            }
            await writeRequest.ExecuteAsync(cancellationToken);

            return Unit.Value;
        }

        private async Task<Dictionary<string, decimal>> CalculateScoresForOrganisations(CreateUserOrganisationMatchCommand request,
            CancellationToken cancellationToken)
        {
            var evaluatedScores = new Dictionary<string, decimal>();
            foreach (var matchingOrganisation in request.MatchingOrganisations.GroupBy(x => x.OrganisationId))
            {
                var matchingRequest = new MatchingRequest
                {
                    UserMatches = request.UserTags,
                    OrganisationMatches = matchingOrganisation.Select(x => x)
                };
                var calculatedScores = new List<decimal>();
                foreach (var matchMaker in _matchMakers)
                {
                    var response = await matchMaker.CalculateMatch(matchingRequest, cancellationToken);
                    calculatedScores.Add(response.Score);
                }

                evaluatedScores.Add(matchingOrganisation.Key, calculatedScores.Average());
            }

            return evaluatedScores;
        }
    }
}
