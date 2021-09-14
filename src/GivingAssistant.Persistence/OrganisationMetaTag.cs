using System.Collections.Generic;

namespace GivingAssistant.Persistence
{
    public class OrganisationMetaTag 
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static OrganisationMetaTag FromKeyValuePair(KeyValuePair<string, string> entry)
        {
            return new() {Name = entry.Key, Value = entry.Value};
        }

        public KeyValuePair<string, string> ToKeyValuePair()
        {
            return new(Name, Value);
        }
    }

    public class QuestionMetaTag
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static QuestionMetaTag FromKeyValuePair(KeyValuePair<string, string> entry)
        {
            return new() {Name = entry.Key, Value = entry.Value};
        }

        public KeyValuePair<string, string> ToKeyValuePair()
        {
            return new(Name, Value);
        }
    }
}