using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace GivingAssistant.Api.Services
{
    public class DatabaseSeedService : BackgroundService
    {
        public const string TableName = "Items";
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly IMediator _mediatr;

        public DatabaseSeedService(IAmazonDynamoDB dynamoDb, IMediator mediatr)
        {
            _dynamoDb = dynamoDb;
            _mediatr = mediatr;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var currentTablesResponse = await _dynamoDb.ListTablesAsync(TableName, 1, stoppingToken);
                if (currentTablesResponse.TableNames.Any())
                    return;

                await _dynamoDb.CreateTableAsync(TableName, new List<KeySchemaElement>
            {
                new("PK", KeyType.HASH),
                new("SK", KeyType.RANGE)
            }, new List<AttributeDefinition>
            {
                new("PK", ScalarAttributeType.S),
                new("SK", ScalarAttributeType.S)
            }, new ProvisionedThroughput(5, 5), stoppingToken);
            }
            catch  (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}