namespace GivingAssistant.Api.Requests.Matches
{
    public class GetUserOrganisationMatchesListRequest
    {
        public string UserId { get; set; }
        public int? MinimumScore { get; set; }
        public int? Limit { get; set; }
    }
}