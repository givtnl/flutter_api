using System;

namespace GivingAssistant.Business.Matches.Infrastructure
{
    public class MatchingResponse
    {
        public MatchingResponse(decimal score, string tag)
        {
            Score = score;
            Tag = tag;
        }
        public decimal Score { get; }
        public string Tag { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as MatchingResponse);
        }
        
        public bool Equals(MatchingResponse other)
        {
            return other != null && Tag == other.Tag;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tag);
        }
    }
}