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
            // step one, calculate the scores for the tags between user and organisation
            var evaluatedScores = CalculateScoresForOrganisations(request);

            var totalScoresWriteRequests = _dynamoDb.CreateBatchWrite<UserOrganisationMatch>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });
            totalScoresWriteRequests.AddPutItems(GenerateTotalMatchScoreForOrganisationAndUser(request, evaluatedScores).ToList());
            var individualTagScoreWriteRequests = _dynamoDb.CreateBatchWrite<UserOrganisationTagMatch>(new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            });
            individualTagScoreWriteRequests.AddPutItems(GenerateIndividualTagScoresForOrganisationAndUser(request, evaluatedScores).ToList());

            await totalScoresWriteRequests.Combine(individualTagScoreWriteRequests).ExecuteAsync(cancellationToken);

            return Unit.Value;
        }

        private IEnumerable<UserOrganisationTagMatch> GenerateIndividualTagScoresForOrganisationAndUser(CreateUserOrganisationMatchCommand request,
            Dictionary<string, MatchingCollection> evaluatedScores)
        {
            foreach (var keyValuePair in evaluatedScores)
            {
                foreach (var matchingResponse in keyValuePair.Value)
                {
                    yield return new UserOrganisationTagMatch
                    {
                        PrimaryKey = $"{Constants.UserPlaceholder}#{request.User}",
                        SortKey = $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TagPlaceholder}#{keyValuePair.Key}#{matchingResponse.Tag}",
                        Score = matchingResponse.Score
                    };
                }
            }
        }

        private IEnumerable<UserOrganisationMatch> GenerateTotalMatchScoreForOrganisationAndUser(CreateUserOrganisationMatchCommand request,
            Dictionary<string, MatchingCollection> evaluatedScores)
        {
            foreach (var keyValuePair in evaluatedScores)
            {
                var organisation = request.MatchingOrganisations
                    .Where(x => x.OrganisationId == keyValuePair.Key)
                    .Select(x => _mapper.Map(x.Organisation, new OrganisationProfile()))
                    .FirstOrDefault();

                if (organisation == null)
                    continue;
                var roundedScore = decimal.Round(keyValuePair.Value.Average(x => x.Score), 2);
                yield return new UserOrganisationMatch
                {
                    Organisation = organisation,
                    Score = roundedScore,
                    PrimaryKey = $"{Constants.UserPlaceholder}#{request.User}",
                    SortKey = $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}#{Constants.TotalScorePlaceHolder}#{roundedScore}#{organisation.Id}"
                };
            }
        }

        private Dictionary<string, MatchingCollection> CalculateScoresForOrganisations(CreateUserOrganisationMatchCommand request)
        {
            var evaluatedScores = new Dictionary<string, MatchingCollection>();
            foreach (var matchingOrganisation in request.MatchingOrganisations.GroupBy(x => x.OrganisationId).ToList())
            {
                var matchingRequest = new MatchingRequest
                {
                    UserMatches = request.UserTags,
                    OrganisationMatches = matchingOrganisation.Select(x => x)
                };
                var calculatedScores = new MatchingCollection();

                foreach (var matchMaker in _matchMakers.OrderBy(x => x.Order))
                {
                    var tagScores = matchMaker.CalculateMatches(matchingRequest, calculatedScores);

                    calculatedScores.AddRange(tagScores.ToList());
                }

                if (calculatedScores.Any())
                    evaluatedScores.Add(matchingOrganisation.Key, calculatedScores);
            }

            return evaluatedScores;
        }
    }
}