using System.Collections.Generic;
using Amazon.Lambda.DynamoDBEvents;
using GivingAssistant.Persistence;

namespace GivingAssistant.UnitTests.Infrastructure
{
    public class SeedModel
    {
        public string Description { get; set; }
        public List<SeedExpectionModel> Expectations { get; set; } = new();
        public List<DynamoDBEvent.DynamodbStreamRecord> Records = new();

        public  DynamoDBEvent ToDynamoDbEvent()
        {
            return new DynamoDBEvent {Records = Records};
        }
    }

    public class SeedExpectionModel
    {
        public string Comment { get; set; }
        public decimal ExpectedScore { get; set; }
        public string OrganisationId { get; set; }
    }
}