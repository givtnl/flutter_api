using System.Collections.Generic;
using GivingAssistant.Persistence;

namespace GivingAssistant.UnitTests.Infrastructure
{
    public class SeedModel
    {
        public List<QuestionTag> QuestionTag { get; set; }
        public List<Question> Question { get; set; }
        public List<Answer> Answer { get; set; }
        public List<UserTagMatch> UserTagMatch { get; set; }
        public List<OrganisationTagMatch> OrganisationTagMatch { get; set; }
    }
}