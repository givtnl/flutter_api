using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace GivingAssistant.Persistence
{
    public class OrganisationProfile : BaseItem
    {
        public OrganisationProfile()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string ImageUrl { get; set; }
        public string Mission { get; set; }
        public string Vision { get; set; }
        public string GivtIdentifier { get; set; }
        public List<OrganisationMetaTag> MetaTags { get; set; } 
    }
}