using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Mappers;
using MediatR;

namespace GivingAssistant.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonDynamoDB>(Configuration.GetAWSOptions("DynamoDb"));
            services.AddTransient<IDynamoDBContext, DynamoDBContext>();

            services.AddAutoMapper(x => x.AddMaps(typeof(QuestionMapper), typeof(Mappers.QuestionMapper)));
            services.AddMediatR(typeof(CreateQuestionCommand));
            services.AddControllers();
            services.AddOpenApiDocument(options =>
            {
          
                options.GenerateEnumMappingDescription = true;
                options.Title = "Giving Assistant API";
                options.AllowNullableBodyParameters = false;
                options.Version = "1";
                options.DocumentName = "v1";
                options.PostProcess = document =>
                {
                    document.Produces = new List<string>
                    {
                        "application/json"
                    };
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3(x =>
            {
                x.AdditionalSettings["displayOperationId"] = true;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
