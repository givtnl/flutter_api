using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Chatbot;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SSM;

namespace GivingAssistant.Infrastructure.AWS
{
    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
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

            var dynamoDbTable = new Table(applicationStack, "ItemsTable", new TableProps
            {
                BillingMode = BillingMode.PAY_PER_REQUEST,
                Encryption = TableEncryption.AWS_MANAGED,
                PartitionKey = new Attribute() {Type = AttributeType.STRING, Name = "PK"},
                SortKey = new Attribute() {Type = AttributeType.STRING, Name = "SK"},
                TableName = "Items",
                RemovalPolicy = RemovalPolicy.DESTROY
            });
            
            var apiGateway = new LambdaRestApi(applicationStack, "Api", new LambdaRestApiProps
            {
                EndpointTypes = new EndpointType[] {EndpointType.REGIONAL},
                Proxy = true,
                Handler = lambdaFunction
            });

            dynamoDbTable.GrantReadWriteData(lambdaFunction);
          

            app.Synth();
        }
    }
}
