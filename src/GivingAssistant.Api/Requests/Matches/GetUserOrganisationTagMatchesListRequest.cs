using NSwag.Annotations;

namespace GivingAssistant.Api.Requests.Matches
{
    public class GetUserOrganisationTagMatchesListRequest
    {
        public string UserId { get; set; }
        [OpenApiIgnore]
        public string OrganisationId { get; set; }
    }
}