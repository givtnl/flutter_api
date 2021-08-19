using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Questions.Commands.Create;
using GivingAssistant.Business.Questions.Queries.GetList;
using GivingAssistant.Domain;
using GivingAssistant.Persistence;
using GivingAssistant.UnitTests.Infrastructure;
using NUnit.Framework;

namespace GivingAssistant.UnitTests
{
    [TestFixture]
    public class QuestionTestFixture : BaseTestFixture
    {

        [Test]
        public async Task EnsureQuestionIsGenerated()
        {
            var commandHandler = new CreateQuestionCommandHandler(DynamoDb, Mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                StatementOptions = new CreateQuestionStatementCommandOptions
                {
                    TagScores = new Dictionary<string, int>
                    {
                        {"Food", 100}
                    },
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);

            Assert.IsNotNull(response.Id);
        }
        [Test]
        public async Task EnsureQuestionsAreRetrievedFromList()
        {
            var commandHandler = new CreateQuestionCommandHandler(DynamoDb, Mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);

            var listResponse = await new GetQuestionsListQueryHandler(DynamoDb, Mapper).Handle(new GetQuestionsListQuery(), CancellationToken.None);

            Assert.IsTrue(listResponse.Any(x => x.Id == response.Id) && listResponse.Count() == 1);
        }
        [Test]
        [TestCase("Food", 60)]
        public async Task EnsureQuestionTagsAreInsertedWithStatements(string tagName, int score)
        {
            var commandHandler = new CreateQuestionCommandHandler(DynamoDb, Mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                StatementOptions = new CreateQuestionStatementCommandOptions
                {
                    TagScores = new Dictionary<string, int>
                    {
                        {tagName, score}
                    }
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);

            var dynamoDbResponse = await DynamoDb.LoadAsync<QuestionMetaData>(Constants.QuestionPlaceholder,
                $"{response.Id}#{Constants.TagPlaceholder}#{tagName}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.IsNotNull(dynamoDbResponse);
        }
        [Test]
        [TestCase("Food", 60)]
        public async Task EnsureQuestionTagsCanBeRetrievedWithStatements(string tagName, int score)
        {
            var commandHandler = new CreateQuestionCommandHandler(DynamoDb, Mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                StatementOptions = new CreateQuestionStatementCommandOptions
                {
                    TagScores = new Dictionary<string, int>
                    {
                        {tagName, score}
                    }
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);

            var dynamoDbResponse = await DynamoDb.LoadAsync<BaseItem>(Constants.QuestionPlaceholder,
                $"{response.Id}#{Constants.TagPlaceholder}#{tagName}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.IsNotNull(dynamoDbResponse);
        }
        [Test]
        [TestCase("Food", 60)]
        public async Task EnsureQuestionTagsCanBeRetrievedWithCategories(string tagName, int score)
        {
            var commandHandler = new CreateQuestionCommandHandler(DynamoDb, Mapper);
            var response = await commandHandler.Handle(new CreateQuestionCommand
            {
                DisplayOrder = 1,
                CategoryOptions = new List<CreateQuestionCategoryCommandOptions>
                {
                 new()
                 {
                     TagScores = new Dictionary<string, int>
                     {
                         {tagName, score}
                     },
                     DisplayOrder = 1,
                     Translations = new Dictionary<string, string>
                     {
                         {"en","option1"}
                     }
                 }
                },
                Translations = new Dictionary<string, string>
                {
                    {
                        "en" , "Do people waste too much food?"},
                    {"nl" , "Verspillen de mensen teveel voedsel?"
                    }
                },
                Type = QuestionType.Statement
            }, CancellationToken.None);

            var dynamoDbResponse = await DynamoDb.LoadAsync<BaseItem>(Constants.QuestionPlaceholder,
                $"{response.Id}#{Constants.TagPlaceholder}#{tagName}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.IsNotNull(dynamoDbResponse);
        }
    }
}