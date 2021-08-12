namespace GivingAssistant.Api.Requests.Matches
{
    public class GetMatchesListRequest
    {
        public string UserId { get; set; }
        public int? MinimumScore { get; set; }
    }
}