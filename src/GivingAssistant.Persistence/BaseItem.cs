using Amazon.DynamoDBv2.DataModel;

namespace GivingAssistant.Persistence
{
    public class BaseItem
    {
        [DynamoDBHashKey("PK")]
        public string PrimaryKey { get; set; }
        [DynamoDBRangeKey("SK")]
        public string SortKey { get; set; }
    }
}