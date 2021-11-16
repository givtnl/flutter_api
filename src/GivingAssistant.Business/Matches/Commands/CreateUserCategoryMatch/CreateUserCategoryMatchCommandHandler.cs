using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using MediatR;

namespace GivingAssistant.Business.Matches.Commands.CreateUserCategoryMatch
{
    public class CreateUserCategoryMatchCommandHandler: IRequestHandler<CreateUserCategoryMatchCommand, Unit>
    {
        private readonly IDynamoDBContext _context;
        private readonly IMapper _mapper;

        public CreateUserCategoryMatchCommandHandler(IDynamoDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<Unit> Handle(CreateUserCategoryMatchCommand request, CancellationToken cancellationToken)
        {
            var existingMatchItem = await _context.LoadAsync<UserCategoryMatch>($"{Constants.UserPlaceholder}#{request.User}",
                $"{Constants.MatchPlaceholder}#{Constants.CategoryPlaceholder}#{request.Category}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                }, cancellationToken
            ) ?? new UserCategoryMatch();

            _mapper.Map(request, existingMatchItem);

            await _context.SaveAsync(existingMatchItem, new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }, cancellationToken);

            return Unit.Value;
        }
    }
}