using System.Collections.Generic;

namespace GivingAssistant.Business.Organisations.Models
{
    public class OrganisationDetailModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Mission { get; set; }
        public string Vision { get; set; }
        public string GivtIdentifier { get; set; }
        public Dictionary<string,string> MetaTags { get; set; }
    }
}