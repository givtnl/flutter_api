using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserTagMatch
{
    public class CreateUserTagMatchCommandHandler : IRequestHandler<CreateUserTagMatchCommand, Unit>
    {
        private readonly IDynamoDBContext _context;
        private readonly IMapper _mapper;

        public CreateUserTagMatchCommandHandler(IDynamoDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Unit> Handle(CreateUserTagMatchCommand request, CancellationToken cancellationToken)
        {
            var existingMatchItem = await _context.LoadAsync<UserTagMatch>($"{Constants.UserPlaceholder}#{request.User}",
                    $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{request.Question.Tag}", new DynamoDBOperationConfig
                    {
                        OverrideTableName = Constants.TableName
                    }, cancellationToken
                    ) ?? new UserTagMatch();

            _mapper.Map(request, existingMatchItem);

            await _context.SaveAsync(existingMatchItem, new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }, cancellationToken);

            return Unit.Value;
        }
    }
}