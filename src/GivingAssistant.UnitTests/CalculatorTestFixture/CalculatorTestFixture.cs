using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.TestUtilities;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Infrastructure;
using GivingAssistant.Business.Matches.Infrastructure.Matchers;
using GivingAssistant.Persistence;
using GivingAssistant.UnitTests.Infrastructure;
using GivingAssistant.UserMatchCalculator;
using GivingAssistant.UserMatchCalculator.Handlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.CalculatorTestFixture
{
    [TestFixture]
    public class CalculatorTestFixture : BaseTestFixture
    {
        private FunctionHandler _handler;
        
        public override async Task Setup()
        {
            await base.Setup();

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
                            await DynamoDb.SaveAsync(serializedRecord.ToObject<OrganisationProfile>(), new DynamoDBOperationConfig{OverrideTableName = Constants.TableName});
                            break;
                        case "METADATA#SCORE":
                            await DynamoDb.SaveAsync(serializedRecord.ToObject<OrganisationTagScore>(), new DynamoDBOperationConfig{OverrideTableName = Constants.TableName});
                            break;
                    }
                    
                    if (sortKeyValue.StartsWith("MATCH#TAG"))
                    {
                        await DynamoDb.SaveAsync(serializedRecord.ToObject<OrganisationTagMatch>(), new DynamoDBOperationConfig{OverrideTableName = Constants.TableName});    
                    }
                }

                if (primaryKeyValue.StartsWith(Constants.QuestionPlaceholder))
                {
                    if (sortKeyValue.StartsWith(Constants.MetaDataPlaceholder))
                        await DynamoDb.SaveAsync(serializedRecord.ToObject<QuestionMetaData>(), new DynamoDBOperationConfig{OverrideTableName = Constants.TableName});

                    if (sortKeyValue.Contains("#TAG#"))
                        await DynamoDb.SaveAsync(serializedRecord.ToObject<QuestionTag>(), new DynamoDBOperationConfig{OverrideTableName = Constants.TableName});
                }
            }

            var matchMakers = new IUserOrganisationMatcher[]
            {
                new BestMatchingTagsMatcher(),
                new NumberOfMatchingTagsMatcher()
            };
            _handler = new FunctionHandler
            {
                Mapper = Mapper,
                DynamoDbContext = DynamoDb,
                MatchMakers = matchMakers,
                Handlers = new IAnsweredQuestionHandler[]
                {
                    new CategoryAnsweredHandler(DynamoDb, Mapper),
                    new StatementAnsweredHandler(DynamoDb, Mapper),
                    new ReCalculateUserTagsAndOrganisationMatchHandler(DynamoDb, Mapper, matchMakers)
                }
             
                
            };
            
        }

        private async Task<SeedModel> SeedDatabaseWithAnswers(string file)
        {
            // read the file
            var fileContents = await File.ReadAllTextAsync(Path.Combine(TestContext.CurrentContext.TestDirectory, "CalculatorTestFixture", file));

            // deserialize to dictionary
            var deserializedContents = JsonConvert.DeserializeObject<SeedModel>(fileContents);

            foreach (var deserializedContentsRecord in deserializedContents.Records)
            {
                await DynamoDb.SaveAsync(new Answer
                {
                    PrimaryKey = deserializedContentsRecord.Dynamodb.NewImage["PK"].S,
                    SortKey = deserializedContentsRecord.Dynamodb.NewImage["SK"].S,
                    Score = decimal.Parse(deserializedContentsRecord.Dynamodb.NewImage["Score"].N, CultureInfo.InvariantCulture)
                }, new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });
            }
            return deserializedContents;
        }
        
        [Test]
        [TestCase("matches-with-only-one-tag.json", "TestFixture")]
        [TestCase("matches-with-only-multiple-tag.json", "TestFixture")]
        public async Task EnsureThereIsAMatchWithAGivenorganisation(string file, string user)
        {
            var seedingModel = await SeedDatabaseWithAnswers(file);
    
            await _handler.HandleAsync(seedingModel.ToDynamoDbEvent(), new TestLambdaContext());
            
            //retrieve the matches
            var matches = await RetrieveRecords<UserOrganisationMatch>($"{Constants.UserPlaceholder}#{user}", $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}");

            foreach (var expectation in seedingModel.Expectations)
            {
                Assert.IsTrue(matches.Any(x => x.Organisation.Id == expectation.OrganisationId),$"{seedingModel.Description} {expectation.Comment}");
            }
        }
        
        [Test]
        [TestCase("matches-with-only-one-tag.json", "TestFixture")]
        [TestCase("matches-with-only-multiple-tag.json", "TestFixture")]
        public async Task EnsureThePercentageMatchesWithExpectations(string file, string user)
        {
            var seedingModel = await SeedDatabaseWithAnswers(file);
    
            await _handler.HandleAsync(seedingModel.ToDynamoDbEvent(), new TestLambdaContext());
            
            //retrieve the matches
            var matches = await RetrieveRecords<UserOrganisationMatch>($"{Constants.UserPlaceholder}#{user}", $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}");

            foreach (var expectation in seedingModel.Expectations)
            {
                Assert.AreEqual(matches.First(x => x.Organisation.Id == expectation.OrganisationId).Score,expectation.ExpectedScore, $"{seedingModel.Description} {expectation.Comment}");
            }
        }
    }
}