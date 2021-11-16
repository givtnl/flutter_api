using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Matches.Commands.CreateUserCategoryMatch;
using GivingAssistant.Domain;
using GivingAssistant.UserMatchCalculator.Models;

namespace GivingAssistant.UserMatchCalculator.Handlers
{
    public class CategoryAnsweredHandler : IAnsweredQuestionHandler
    {
        private readonly IDynamoDBContext _context;
        private readonly IMapper _mapper;

        public CategoryAnsweredHandler(IDynamoDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public int ExecutionOrder => 1;
        
        public async Task Handle(HandleAnsweredQuestionRequest request)
        {
            var createUserCategoryMatchCommandHandler = new CreateUserCategoryMatchCommandHandler(_context, _mapper);
            foreach (var questionTagListModel in request.QuestionTags)
            {
                if (!questionTagListModel.Tag.Equals(request.AnsweredTag, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                
                request.LambdaContext.Logger.LogLine($"Creating Match for user {request.User} with tag {questionTagListModel.Tag}(QUESTIONTAGSCORE:{questionTagListModel.Score})");
                await createUserCategoryMatchCommandHandler.Handle(new CreateUserCategoryMatchCommand
                {
                    User = request.User,
                    Category = request.AnsweredTag
                }, CancellationToken.None);
            }
        }
        
        public bool CanHandle(HandleAnsweredQuestionRequest request)
        {
            return request.AnsweredQuestion.Type == QuestionType.Category;
        }
    }
}