using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Organisations.Commands.Create;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Persistence;
using NUnit.Framework;

namespace GivingAssistant.UnitTests
{
    [TestFixture]
    public class OrganisationTestFixture
    {
        private IAmazonDynamoDB _dynamoDb;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _dynamoDb = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
            {
                ServiceURL = "http://localhost:8000"
            });
            _mapper = new MapperConfiguration(x => x.AddMaps(typeof(QuestionMapper).Assembly)).CreateMapper();

        }
        [TearDown]
        public async Task ClearDatabase()
        {
            var toDeleteItems = await _dynamoDb.ScanAsync(new ScanRequest(Constants.TableName));
            foreach (var deleteItem in toDeleteItems.Items)
            {
                await _dynamoDb.DeleteItemAsync(Constants.TableName, new Dictionary<string, AttributeValue>
                {
                    {"SK", new AttributeValue(deleteItem["SK"].S)},
                    {"PK", new AttributeValue(deleteItem["PK"].S)}
                });
            }
        }

        [Test]
        public async Task EnsureOrganisationProfileIsInserted()
        {
            var createOrganisationCommand = new CreateOrganisationCommand
            {
                Name = "Amnesty International",
                Description = "Amnesty doet onderzoek, spreekt autoriteiten aan op mensenrechtenschendingen en roept hen publiekelijk ter verantwoording. Dat doen we namens al onze leden en schenkers. Hun betrokkenheid en verontwaardiging zet Amnesty om in daden. ",
                ImageUrl = "https://www.amnesty-international.be/sites/all/themes/amnesty2016/images/logo-land.png",
                Mission = "Mensenrechten verdedig je met vuur!",
                Vision = "Samen kunnen we onrecht stoppen. De vrijheid eisen van mensen die vastzitten om wie ze zijn of waarin ze geloven. De rechten van vrouwen en meisjes beter beschermen. Een einde maken aan folteringen en de doodstraf. Jouw gift maakt een structureel verschil in het leven van mensen, dichtbij én veraf.",
                WebsiteUrl = "https://www.amnesty-international.be/",
                TagScores = new Dictionary<string, int>
                {
                    {"Internationale hulp en mensenrechten", 100},
                    {"Mensenrechten", 80},
                    {"Internationaal", 80},
                },
                MetaTags = new Dictionary<string, string>
                {
                    {"MetaDataOne","valueone"},
                    {"MetaDataTwo","valuetwo"},
                    {"MetaDataThree","valuethree"}
                }
            };

            var response = await new CreateOrganisationCommandHandler(_dynamoDb, _mapper).Handle(createOrganisationCommand, CancellationToken.None);
            await new DynamoDBContext(_dynamoDb).LoadAsync<OrganisationProfile>($"{Constants.OrganisationPlaceholder}#{response.Id}",
                $"{Constants.MetaDataPlaceholder}#{Constants.ProfilePlaceholder}", new DynamoDBOperationConfig
                {
                    OverrideTableName = Constants.TableName
                });

            Assert.AreEqual(response.Name, createOrganisationCommand.Name);
            Assert.AreEqual(response.ImageUrl, createOrganisationCommand.ImageUrl);
            Assert.AreEqual(response.Description, createOrganisationCommand.Description);
            Assert.AreEqual(response.MetaTags, createOrganisationCommand.MetaTags);
            Assert.AreEqual(response.Mission, createOrganisationCommand.Mission);
            Assert.AreEqual(response.Vision, createOrganisationCommand.Vision);
        }

    }
}