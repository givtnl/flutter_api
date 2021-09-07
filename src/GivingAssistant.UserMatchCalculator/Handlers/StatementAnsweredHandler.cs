using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Matches.Commands.CreateUserTagMatch;
using GivingAssistant.Domain;
using GivingAssistant.UserMatchCalculator.Models;

namespace GivingAssistant.UserMatchCalculator.Handlers
{
    public class StatementAnsweredHandler : IAnsweredQuestionHandler
    {
        private readonly IDynamoDBContext _context;
        private readonly IMapper _mapper;

        public StatementAnsweredHandler(IDynamoDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public int ExecutionOrder => 1;

        public async Task Handle(HandleAnsweredQuestionRequest request)
        {
            await SaveUserMatchesWithTags(request);

          //  await SaveUserMatchesWithOrganisation(request);
            
        }

        private async Task SaveUserMatchesWithTags(HandleAnsweredQuestionRequest request)
        {
            var createUserTagMatchCommandHandler = new CreateUserTagMatchCommandHandler(_context, _mapper);
            foreach (var questionTagListModel in request.QuestionTags)
            {
                request.LambdaContext.Logger.LogLine($"Creating Match for user {request.User} with tag {questionTagListModel.Tag}(QUESTIONTAGSCORE:{questionTagListModel.Score})");
                await createUserTagMatchCommandHandler.Handle(new CreateUserTagMatchCommand
                {
                    User = request.User,
                    Answer = request.Answer,
                    Question = questionTagListModel
                }, CancellationToken.None);
            }
        }

        // private async Task SaveUserMatchesWithOrganisation(HandleAnsweredQuestionRequest request)
        // {
        //     var matchingOrganisations = await new GetOrganisationsByTagsListQueryHandler(_context, _mapper).Handle(new GetOrganisationsByTagsListQuery
        //     {
        //         Tags =request.QuestionTags.Select(x => x.Tag)
        //     }, CancellationToken.None);
        //     
        //     var userTags = await new GetMatchesWithTagsListQueryHandler(_context, _mapper).Handle(new GetMatchesWithTagsListQuery {UserId = request.User}, new CancellationToken());
        //
        //     await new CreateUserOrganisationMatchCommandHandler(_context, _mapper, _matchMakers).Handle(
        //         new CreateUserOrganisationMatchCommand
        //         {
        //             User = request.User,
        //             MatchingOrganisations = matchingOrganisations,
        //             UserTags = userTags
        //         }, CancellationToken.None);
        // }

        public bool CanHandle(HandleAnsweredQuestionRequest request)
        {
            return request.AnsweredQuestion.Type == QuestionType.Statement;
        }
    }
}