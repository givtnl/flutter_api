namespace GivingAssistant.Business.Matches.Infrastructure
{
    public class MatchingResponse
    {
        public MatchingResponse(decimal score)
        {
            Score = score;
        }
        public decimal Score { get; }

        public static MatchingResponse EmptyMatch()
        {
            return new MatchingResponse(decimal.Zero);
        }
    }
}