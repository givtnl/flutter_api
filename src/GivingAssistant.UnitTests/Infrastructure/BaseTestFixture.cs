using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Persistence;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.Infrastructure
{
    public class BaseTestFixture
    {
        protected IDynamoDBContext DynamoDb;
        protected IMapper Mapper;

        [SetUp]
        public virtual Task Setup()
        {
            DynamoDb = new DynamoDBContext(new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            }));
            Mapper = new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();
            
            return Task.CompletedTask;
        }

        protected async Task<List<T>> RetrieveRecords<T>(string primaryKey, string sortingKey)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, primaryKey);
            filter.AddCondition("SK", QueryOperator.BeginsWith, sortingKey);
            
           return await DynamoDb
                .FromQueryAsync<T>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig { OverrideTableName = Constants.TableName }).GetRemainingAsync();
        }
        
        [TearDown]
        public async Task ClearDatabase()
        {
            var toDeleteItems = await DynamoDb.ScanAsync<BaseItem>(Enumerable.Empty<ScanCondition>(),
                new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                }).GetRemainingAsync();

            foreach (var deleteItem in toDeleteItems)
            {
                await DynamoDb.DeleteAsync(deleteItem, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }
        }
    }
}