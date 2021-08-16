using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using AutoMapper;
using GivingAssistant.Business.Answers.Commands.Create;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Business.Matches.Commands.Create;
using GivingAssistant.Business.Matches.Queries.GetMatchesWithOrganisationsList;
using GivingAssistant.Business.Organisations.Models;
using GivingAssistant.Business.Questions.Mappers;
using GivingAssistant.Persistence;
using NUnit.Framework;

namespace GivingAssistant.UnitTests
{
    [TestFixture]
    public class MatchTestFixture
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
        [Test]
        [TestCase("Anthony")]
        public async Task EnsureMatchesArePersisted(string userId)
        {
            var commandHandler = new CreateMatchCommandHandler(_dynamoDb, _mapper);
            await commandHandler.Handle(new CreateMatchCommand
            {
                User = userId,
                MatchingOrganisations = new[]{
                  new OrganisationTagMatchListModel
                  {
                      OrganisationId = "org-2",
                      Score = 80,
                      Tag = "black",
                      Organisation = new OrganisationDetailModel
                      {
                          Id = "org-2"
                          
                      }
                  },
                  new OrganisationTagMatchListModel
              {
                  OrganisationId = "org-1",
                  Organisation = new OrganisationDetailModel
                  {
                      Id = "org-1"
                  },
                  Score = 20,
                  Tag = "yellow"
              }}
            }, CancellationToken.None);

            var response = await new GetMatchesWithOrganisationsListQueryHandler(_dynamoDb, _mapper).Handle(new GetMatchesWithOrganisationsListQuery() { UserId = userId }, CancellationToken.None);

            Assert.IsTrue(response.Any(x => x.Organisation.Id == "org-1" && x.Score == 20));
            Assert.IsTrue(response.Any(x => x.Organisation.Id == "org-2" && x.Score == 80));
        }
    }
}