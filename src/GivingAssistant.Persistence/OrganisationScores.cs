using System.Collections.Generic;

namespace GivingAssistant.Persistence
{
    public class OrganisationTagScoreDetail
    {
        public static OrganisationTagScoreDetail FromDictionaryEntry(KeyValuePair<string,int> entry)
        {
            return new() {Tag = entry.Key, Score = entry.Value};
        }
     
        public string Tag { get; set; }
        public int Score { get; set; }
    }
}