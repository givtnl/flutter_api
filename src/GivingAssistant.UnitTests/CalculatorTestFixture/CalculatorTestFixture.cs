using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.TestUtilities;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Persistence;
using GivingAssistant.UnitTests.Infrastructure;
using GivingAssistant.UserMatchCalculator;
using NUnit.Framework;

namespace GivingAssistant.UnitTests.CalculatorTestFixture
{
    [TestFixture]
    public class CalculatorTestFixture : BaseTestFixture
    {
        /// <summary>
        /// Expected is that Nico has a match with Dorcas because he answered a question in favor of animals
        /// Expected is that Anthony has no matches because no organisation matches with nature has been found
        /// </summary>
        /// <returns></returns>
        [Test]
        [TestCase("TestCaseOne.json", "Nico", "AnimalsQuestion", "Dorcas", "animals", 1)]
        [TestCase("TestCaseOne.json", "Anthony", "ReligionQuestion", "PijlerLelyStad", "religion", 1)]
        public async Task EnsureUserOrganisationMatchExpectations(string file, string user, string questionId, string matchedOrganisation, string matchingTag, int numberOfMatches)
        {
            await LoadItems(Path.Combine(TestContext.CurrentContext.WorkDirectory, nameof(CalculatorTestFixture), file));
            var handler = new FunctionHandler { DynamoDbContext = DynamoDb, Mapper = Mapper };
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
                                {Constants.SortKeyPlaceHolder, new AttributeValue($"ANSWER#QUESTION#{questionId}#{matchingTag}")},
                                {Constants.ScorePlaceholder, new AttributeValue{N = "10"}}
                            }
                        }
                    }
                }
            }, new TestLambdaContext());
            var filter = new QueryFilter();
            filter.AddCondition("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{user}");
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.OrganisationPlaceholder}");
            var queryResults = await DynamoDb.FromQueryAsync<BaseItem>(new QueryOperationConfig
            {
                Filter = filter
            }, new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }).GetRemainingAsync();

            Assert.AreEqual(numberOfMatches, queryResults.Count(p => p.SortKey.Contains(matchedOrganisation) && p.SortKey.EndsWith(matchingTag)));
        }


        [Test]
        [TestCase("TestCaseOne.json", "Nico", "AnimalsQuestion", "animals", 100, 80, 80)]
        [TestCase("TestCaseOne.json", "Anthony", "ReligionQuestion", "religion", 25, 40, 10)]
        public async Task EnsureUserProfileIsUpdated(string file, string user, string questionId, string matchingTag, decimal expectedScorePercentage, int maxScore, int currentScore)
        {
            await LoadItems(Path.Combine(TestContext.CurrentContext.WorkDirectory, nameof(CalculatorTestFixture), file));
            var handler = new FunctionHandler { DynamoDbContext = DynamoDb, Mapper = Mapper };
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
                                {Constants.SortKeyPlaceHolder, new AttributeValue($"ANSWER#QUESTION#{questionId}#{matchingTag}")},
                                {Constants.ScorePlaceholder, new AttributeValue{N = currentScore.ToString()}}
                            }
                        }
                    }
                }
            }, new TestLambdaContext());
            var filter = new QueryFilter();
            filter.AddCondition("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{user}");
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{matchingTag}");
            var queryResults = await DynamoDb.FromQueryAsync<UserTagMatch>(new QueryOperationConfig
            {
                Filter = filter,
                Limit = 1
            }, new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }).GetRemainingAsync();

            Assert.AreEqual(1, queryResults.Count);
            Assert.AreEqual(queryResults.First().Tag, matchingTag);
            Assert.AreEqual(queryResults.First().MaximumScore, maxScore);
            Assert.AreEqual(queryResults.First().CurrentScore, currentScore);
            Assert.AreEqual(queryResults.First().Percentage, expectedScorePercentage);
        }

        [Test]
        [TestCase("TestCaseOne.json", "Nico", "animals", 61, 180, 110)]
       
        public async Task EnsureUserProfileIsUpdatedWithMultipleAnswers(string file, string user, string matchingTag, decimal expectedScorePercentage, int maxScore, int currentScore)
        {
            await LoadItems(Path.Combine(TestContext.CurrentContext.WorkDirectory, nameof(CalculatorTestFixture), file));
            var handler = new FunctionHandler { DynamoDbContext = DynamoDb, Mapper = Mapper };
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
                                {Constants.SortKeyPlaceHolder, new AttributeValue($"ANSWER#QUESTION#AnimalsQuestion#{matchingTag}")},
                                {Constants.ScorePlaceholder, new AttributeValue{N = "80"}}
                            }
                        }
                    },
                    new()
                    {
                        Dynamodb = new StreamRecord
                        {
                            NewImage = new Dictionary<string, AttributeValue>
                            {
                                {Constants.PrimaryKeyPlaceHolder, new AttributeValue($"USER#{user}")},
                                {Constants.SortKeyPlaceHolder, new AttributeValue($"ANSWER#QUESTION#AnimalsQuestionTwo")},
                                {Constants.ScorePlaceholder, new AttributeValue{N = "30"}}
                            }
                        }
                    }
                }
            }, new TestLambdaContext());
            var filter = new QueryFilter();
            filter.AddCondition("PK", QueryOperator.Equal, $"{Constants.UserPlaceholder}#{user}");
            filter.AddCondition("SK", QueryOperator.BeginsWith, $"{Constants.MatchPlaceholder}#{Constants.TagPlaceholder}#{matchingTag}");
            var queryResults = await DynamoDb.FromQueryAsync<UserTagMatch>(new QueryOperationConfig
            {
                Filter = filter,
                Limit = 1
            }, new DynamoDBOperationConfig
            {
                OverrideTableName = Constants.TableName
            }).GetRemainingAsync();

            Assert.AreEqual(1, queryResults.Count);
            Assert.AreEqual(queryResults.First().Tag, matchingTag);
            Assert.AreEqual(queryResults.First().MaximumScore, maxScore);
            Assert.AreEqual(queryResults.First().CurrentScore, currentScore);
            Assert.AreEqual(queryResults.First().Percentage, expectedScorePercentage);
        }

    }
}