using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Hosting;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace GivingAssistant.Api
{
    public class LambdaFunctionHandler : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>();
        }
    }
}