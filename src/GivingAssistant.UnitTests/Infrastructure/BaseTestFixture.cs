using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
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
        public void Setup()
        {
            DynamoDb = new DynamoDBContext(new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            }));
            Mapper = new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();
        }

        protected async Task LoadItems(string file)
        {
            var fileContents = await File.ReadAllTextAsync(file);
            var contents = JsonConvert.DeserializeObject<SeedModel>(fileContents);

            foreach (var question in contents.Question)
            {
                await DynamoDb.SaveAsync(question, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }

            foreach (var answer in contents.Answer)
            {
                await DynamoDb.SaveAsync(answer, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }

            foreach (var userTagMatch in contents.UserTagMatch)
            {
                await DynamoDb.SaveAsync(userTagMatch, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }

            foreach (var organisationTagMatch in contents.OrganisationTagMatch)
            {
                await DynamoDb.SaveAsync(organisationTagMatch, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }

            foreach (var questionTag in contents.QuestionTag)
            {
                await DynamoDb.SaveAsync(questionTag, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }
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