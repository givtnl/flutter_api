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
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.Infrastructure
{
    public class BaseTestFixture
    {
        protected IDynamoDBContext DynamoDb;
        protected IMapper Mapper;

        [SetUp]
        public virtual async Task Setup()
        {
            DynamoDb = new DynamoDBContext(new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            }));
            Mapper = new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();

            // read the file
            var fileContents = await File.ReadAllTextAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "CalculatorTestFixture", "DatabaseState.json"));

            // deserialize to dictionary
            var deserializedContents = JsonConvert.DeserializeObject<List<JObject>>(fileContents);

            // we need to loop and decide to what we want to map
            foreach (var serializedRecord in deserializedContents)
            {
                var primaryKeyValue = serializedRecord.Value<string>("PrimaryKey");
                var sortKeyValue = serializedRecord.Value<string>("SortKey");

                if (primaryKeyValue.StartsWith(Constants.OrganisationPlaceholder))
                {
                    switch (sortKeyValue)
                    {
                        case "METADATA#PROFILE":
                            await DynamoDb.SaveAsync(serializedRecord.ToObject<OrganisationProfile>(), new DynamoDBOperationConfig {OverrideTableName = Constants.TableName});
                            break;
                        case "METADATA#SCORE":
                            await DynamoDb.SaveAsync(serializedRecord.ToObject<OrganisationTagScore>(), new DynamoDBOperationConfig {OverrideTableName = Constants.TableName});
                            break;
                    }

                    if (sortKeyValue.StartsWith("MATCH#TAG"))
                    {
                        await DynamoDb.SaveAsync(serializedRecord.ToObject<OrganisationTagMatch>(), new DynamoDBOperationConfig {OverrideTableName = Constants.TableName});
                    }
                }

                if (primaryKeyValue.StartsWith(Constants.QuestionPlaceholder))
                {
                    if (sortKeyValue.StartsWith(Constants.MetaDataPlaceholder))
                        await DynamoDb.SaveAsync(serializedRecord.ToObject<QuestionMetaData>(), new DynamoDBOperationConfig {OverrideTableName = Constants.TableName});

                    if (sortKeyValue.Contains("#TAG#"))
                        await DynamoDb.SaveAsync(serializedRecord.ToObject<QuestionTag>(), new DynamoDBOperationConfig {OverrideTableName = Constants.TableName});
                }
            }
        }

        protected async Task<List<T>> RetrieveRecords<T>(string primaryKey, string sortingKey)
        {
            var filter = new QueryFilter("PK", QueryOperator.Equal, primaryKey);
            filter.AddCondition("SK", QueryOperator.BeginsWith, sortingKey);

            return await DynamoDb
                .FromQueryAsync<T>(new QueryOperationConfig
                {
                    Filter = filter
                }, new DynamoDBOperationConfig {OverrideTableName = Constants.TableName}).GetRemainingAsync();
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