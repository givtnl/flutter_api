using System.Collections.Generic;

namespace GivingAssistant.Persistence
{
    public class OrganisationTagScore : BaseItem
    {
        public OrganisationTagScore()
        {
            
        }
        public OrganisationTagScore(string organisationId)
        {
            OrganisationId = organisationId;
        }
        public string OrganisationId { get; set; }
        public List<OrganisationTagScoreDetail> Scores { get; set; }

    }
}