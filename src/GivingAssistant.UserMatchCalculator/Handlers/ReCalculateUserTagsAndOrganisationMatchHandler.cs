using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using GivingAssistant.Business.Matches.Commands.CreateUserOrganisationMatch;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithTagsList;
using GivingAssistant.Business.Organisations.Queries.GetByTags;
using GivingAssistant.UserMatchCalculator.Models;

namespace GivingAssistant.UserMatchCalculator.Handlers
{
    public class ReCalculateUserTagsAndOrganisationMatchHandler : IAnsweredQuestionHandler
    {
        private readonly IDynamoDBContext _context;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IUserOrganisationMatcher> _matchMakers;
        public int ExecutionOrder => int.MaxValue;

        public ReCalculateUserTagsAndOrganisationMatchHandler(IDynamoDBContext context, IMapper mapper, IEnumerable<IUserOrganisationMatcher> matchMakers)
        {
            _context = context;
            _mapper = mapper;
            _matchMakers = matchMakers;
        }

        public async Task Handle(HandleAnsweredQuestionRequest request)
        {
            // retrieve the tags for the users
            var userTags = await new GetMatchesWithTagsListQueryHandler(_context, _mapper).Handle(new GetMatchesWithTagsListQuery {UserId = request.User}, new CancellationToken());

            // retrieve all the organisations that matches these tags
            var organisationMatches = await new GetOrganisationsByTagsListQueryHandler(_context, _mapper).Handle(new GetOrganisationsByTagsListQuery
            {
                Tags = userTags.Select(x => x.Tag)
            }, CancellationToken.None);

            await new CreateUserOrganisationMatchCommandHandler(_context, _mapper, _matchMakers).Handle(
                new CreateUserOrganisationMatchCommand
                {
                    User = request.User,
                    MatchingOrganisations = organisationMatches.Where(x => userTags.Any(userTag => userTag.Tag.Equals(x.Tag, StringComparison.InvariantCultureIgnoreCase))),
                    UserTags = userTags
                }, CancellationToken.None);
            
            
        }

        public bool CanHandle(HandleAnsweredQuestionRequest request)
        {
            return true;
        }
    }
}