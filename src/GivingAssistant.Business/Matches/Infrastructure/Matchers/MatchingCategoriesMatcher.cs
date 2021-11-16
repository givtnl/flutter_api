using System.Collections.Generic;
using System.Linq;

namespace GivingAssistant.Business.Matches.Infrastructure.Matchers
{
    public class MatchingCategoriesMatcher: IUserOrganisationMatcher
    {
        public int Order => 2;
        public IEnumerable<MatchingResponse> CalculateMatches(MatchingRequest context, IEnumerable<MatchingResponse> currentResponses)
        {
            var userCategories = context.UserCategories;
            var organisationCategories = context.OrganisationMatchesByCategories;
            
            foreach (var userCategoryMatchListModel in userCategories)
            {
                if (organisationCategories.Any(x => x.Category == userCategoryMatchListModel.Category))
                {
                    yield return new MatchingResponse(100, userCategoryMatchListModel.Category);
                }
            }
        }
    }
}