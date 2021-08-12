using NJsonSchema.Annotations;

namespace GivingAssistant.Api.Requests.Organisations
{
    public class CreateOrganisationResponse
    {
        [NotNull]
        public string Id { get; set; }
    }
}