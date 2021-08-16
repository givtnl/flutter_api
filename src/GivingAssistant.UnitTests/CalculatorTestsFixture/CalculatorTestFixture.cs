using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.TestUtilities;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Persistence;
using GivingAssistant.UserMatchCalculator;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.CalculatorTestsFixture
{
    [TestFixture]
    public class CalculatorTestFixture
    {
        private IDynamoDBContext _dynamoDb;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _dynamoDb = new DynamoDBContext(new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            }));
            _mapper = new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();

        }
        [TearDown]
        public async Task ClearDatabase()
        {
            var toDeleteItems = await _dynamoDb.ScanAsync<BaseItem>(Enumerable.Empty<ScanCondition>(),
                new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                }).GetRemainingAsync();

            foreach (var deleteItem in toDeleteItems)
            {
                await _dynamoDb.DeleteAsync(deleteItem, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }
        }

        private async Task LoadItems(string file)
        {
            var fileContents = await File.ReadAllTextAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "CalculatorTestsFixture", file));
            var contents = JsonConvert.DeserializeObject<List<BaseItem>>(fileContents);

            foreach (var baseItem in contents)
            {
                await _dynamoDb.SaveAsync(baseItem, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }
        }

        /// <summary>
        /// Expected is that Nico has a match with Dorcas because he answered a question in favor of animals
        /// Expected is that Anthony has no matches because no organisation matches with nature has been found
        /// </summary>
        /// <returns></returns>
        [Test]
        [TestCase("TestCaseOne.json", "Nico", "AnimalsQuestion", "Dorcas", "animals", 1)]
        [TestCase("TestCaseOne.json", "Anthony", "ReligionQuestion", "PijlerLelyStad", "religion", 1)]
        public async Task EnsureTestCaseFitsExpectations(string file, string user, string questionId, string matchedOrganisation, string matchingTag, int numberOfMatches)
        {
            await LoadItems(file);
            var handler = new FunctionHandler { DynamoDbContext = _dynamoDb, Mapper = _mapper };
            await handler.HandleAsync(new DynamoDBEvent
            {
                Records = new List<DynamoDBEvent.DynamodbStreamRecord>
                {
                    new()
                    {
                        Dynamodb = new StreamRecord
                        {
                            NewImage = new Dictionary<string, AttributeValue>
                            {
                                {Constants.PrimaryKeyPlaceHolder, new AttributeValue($"USER#{user}")},
                                {Constants.SortKeyPlaceHolder, new AttributeValue($"ANSWER#QUESTION#{questionId}")}
                            }
                        }
                    }
                }
            }, new TestLambdaContext());
            var filter = new QueryFilter();
            filter.AddCondition("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{user}");
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}");
            var queryResults = await _dynamoDb.FromQueryAsync<BaseItem>(new QueryOperationConfig
            {
                Filter = filter
            }, new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }).GetRemainingAsync();

            Assert.AreEqual(numberOfMatches, queryResults.Count(p => p.SortKey.Contains(matchedOrganisation) && p.SortKey.EndsWith(matchingTag)));
        }

    }
}