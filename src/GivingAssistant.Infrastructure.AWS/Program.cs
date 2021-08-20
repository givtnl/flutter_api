using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.EventSources;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SSM;

namespace GivingAssistant.Infrastructure.AWS
{
    sealed class Program
    {
        private static Table DynamoDbApplicationTable { get; set; }
        private static LambdaRestApi ApiGateway { get; set; }
        public static void Main(string[] args)
        {
            var app = new App();
            var applicationStack = CreateApi(app);
            CreateCalculateUserMatchesStreamListener(app);
            SetupMixPanelProxy(applicationStack);
            app.Synth();
        }

        private static void SetupMixPanelProxy(Stack parentStack)
        {
            var proxyResource = ApiGateway.Root.AddResource("tracking");

            proxyResource.AddMethod("POST", new HttpIntegration("https://api.mixpanel.com/import", new HttpIntegrationProps
            {
                HttpMethod = "POST",
                Proxy = false,
                Options = new IntegrationOptions
                {
                    RequestParameters = new Dictionary<string, string>
                 {
                     {"integration.request.header.Content-Type", "'application/json'"},
                     {"integration.request.header.Authorization", $"'{StringParameter.ValueForStringParameter(parentStack, "MixPanelAuthHeader")}'"},
                     {"integration.request.querystring.strict","'1'"},
                     {"integration.request.querystring.project_id",$"'{StringParameter.ValueForStringParameter(parentStack, "MixPanelProjectId")}'"}
                 },

                    IntegrationResponses = new[]
                        {
                        new IntegrationResponse
                    {
                        SelectionPattern = "2\\d{2}",
                        StatusCode = "200",

                        ResponseParameters = new Dictionary<string, string>
                        {
                            {"method.response.header.Access-Control-Allow-Headers" , "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'"},
                            {"method.response.header.Access-Control-Allow-Origin" , "'*'"},
                            {"method.response.header.Access-Control-Allow-Methods" , "'POST,GET,OPTIONS'"}
                        }
                    }}
                }
            }), new MethodOptions
            {
                MethodResponses = new[]{new MethodResponse
                {
                    StatusCode = "200",
                    ResponseModels = new Dictionary<string, IModel>
                    {
                        {"application/json", Model.EMPTY_MODEL}
                    },
                    ResponseParameters = new Dictionary<string, bool>
                    {
                        {  "method.response.header.Access-Control-Allow-Headers", true},
                      {   "method.response.header.Access-Control-Allow-Methods", true},
                       {  "method.response.header.Access-Control-Allow-Origin",true}
                    }
}}
            });
        }


        private static Stack CreateApi(App app)
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
                Timeout = Duration.Seconds(10),
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

            ApiGateway = new LambdaRestApi(applicationStack, "Api", new LambdaRestApiProps
            {
                EndpointTypes = new EndpointType[] { EndpointType.REGIONAL },
                Proxy = true,
                Handler = lambdaFunction
            });

            DynamoDbApplicationTable.GrantFullAccess(lambdaFunction);

            return applicationStack;
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
            lambdaFunction.AddEventSource(new DynamoEventSource(DynamoDbApplicationTable, new DynamoEventSourceProps
            {
                BatchSize = 10,
                StartingPosition = StartingPosition.TRIM_HORIZON
            }));

            DynamoDbApplicationTable.GrantFullAccess(lambdaFunction);
            
        }
    }
}
