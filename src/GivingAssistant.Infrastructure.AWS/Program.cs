using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Chatbot;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SSM;

namespace GivingAssistant.Infrastructure.AWS
{
    sealed class Program
    {
        private static Table DynamoDbApplicationTable { get; set; }
        public static void Main(string[] args)
        {
            var app = new App();
            CreateApi(app);
            CreateCalculateUserMatchesStreamListener(app);
            app.Synth();
        }

        private static void CreateApi(App app)
        {
            var applicationName = "giving-assistant".ToLower();
            var infrastructureStack = new Stack(app, $"{applicationName}-infrastructure");
            var dockerRepository = new Repository(infrastructureStack, "Repository", new RepositoryProps
            {
                RemovalPolicy = RemovalPolicy.DESTROY,
                LifecycleRules = new ILifecycleRule[]
               {
                   new LifecycleRule
                   {
                       MaxImageCount = 2,
                       TagStatus = TagStatus.ANY
                   }
               }
            });

            var applicationStack = new Stack(app, $"{applicationName}-application");

            var lambdaFunction = new Function(applicationStack, "LambdaFunction", new FunctionProps
            {
                Handler = Handler.FROM_IMAGE,
                Code = Code.FromEcrImage(dockerRepository),
                MemorySize = 4096,
                Runtime = Runtime.FROM_IMAGE,
                LogRetention = RetentionDays.ONE_DAY,
                FunctionName = "giving-assistant-api",
                ReservedConcurrentExecutions = 10
            });

            DynamoDbApplicationTable = new Table(applicationStack, "ItemsTable", new TableProps
            {
                Stream = StreamViewType.NEW_AND_OLD_IMAGES,
                BillingMode = BillingMode.PAY_PER_REQUEST,
                Encryption = TableEncryption.AWS_MANAGED,
                PartitionKey = new Attribute() { Type = AttributeType.STRING, Name = "PK" },
                SortKey = new Attribute() { Type = AttributeType.STRING, Name = "SK" },
                TableName = "Items",
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            var apiGateway = new LambdaRestApi(applicationStack, "Api", new LambdaRestApiProps
            {
                EndpointTypes = new EndpointType[] { EndpointType.REGIONAL },
                Proxy = true,
                Handler = lambdaFunction
            });

            DynamoDbApplicationTable.GrantReadWriteData(lambdaFunction);
        }

        private static void CreateCalculateUserMatchesStreamListener(App app)
        {
            var applicationName = "giving-assistant-user-match-calculator".ToLower();
            var infrastructureStack = new Stack(app, $"{applicationName}-infrastructure");
            var dockerRepository = new Repository(infrastructureStack, "Repository", new RepositoryProps
            {
                RemovalPolicy = RemovalPolicy.DESTROY,
                LifecycleRules = new ILifecycleRule[]
               {
                   new LifecycleRule
                   {
                       MaxImageCount = 2,
                       TagStatus = TagStatus.ANY
                   }
               }
            });

            var applicationStack = new Stack(app, $"{applicationName}-application");

            var lambdaFunction = new Function(applicationStack, "LambdaFunction", new FunctionProps
            {
                Handler = Handler.FROM_IMAGE,
                Code = Code.FromEcrImage(dockerRepository),
                MemorySize = 1024,
                Timeout = Duration.Minutes(1),
                Runtime = Runtime.FROM_IMAGE,
                LogRetention = RetentionDays.ONE_DAY,
                FunctionName = "giving-assistant-user-match-calculator",
                ReservedConcurrentExecutions = 1
            });
            //lambdaFunction.AddEventSource(new DynamoEventSource(DynamoDbApplicationTable, new DynamoEventSourceProps
            //{
            //    BatchSize = 10,
            //    StartingPosition = StartingPosition.TRIM_HORIZON
            //}));
       
            //DynamoDbApplicationTable.GrantReadWriteData(lambdaFunction);
            //DynamoDbApplicationTable.GrantStreamRead(lambdaFunction);
        }
    }
}
