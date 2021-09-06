using System.Collections.Generic;
using NJsonSchema.Annotations;

namespace GivingAssistant.Api.Requests.Organisations
{
    public class CreateOrganisationRequest
    {
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string ImageUrl { get; set; }
        [NotNull]
        public string Mission { get; set; }
        [NotNull]
        public string Vision { get; set; }
        public Dictionary<string, string> MetaTags { get; set; }
        [NotNull]
        public Dictionary<string, int> TagScores { get; set; }
    }
}