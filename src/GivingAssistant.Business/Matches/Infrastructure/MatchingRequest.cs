using System.Collections.Generic;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Persistence;

namespace GivingAssistant.Business.Matches.Infrastructure
{
    public class MatchingRequest
    {
        public string User { get; set; }
        public OrganisationDetailModel Organisation { get; set; }
        public List<UserTagMatch> UserMatches { get; set; }
        public List<OrganisationTagMatch> OrganisationMatches { get; set; }
    }
}