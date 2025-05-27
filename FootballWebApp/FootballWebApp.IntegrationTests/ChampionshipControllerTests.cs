using AutoMapper;
using FootballWebApp.Championship;
using FootballWebApp.Championship.Contract;
using FootballWebApp.Data;
using FootballWebApp.Data.Championship;
using FootballWebApp.Model.Championship;
using FootballWebApp.Model.Common;
using FootballWebApp.Orchestrator.Championship;
using FootballWebApp.User.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace FootballWebApp.IntegrationTests;

public class ChampionshipControllerTests
{
    private ChampionshipsController _championshipsController;
    private IConfiguration _configuration;
    private CosmosDbContext _context;
    private ChampionshipDao _existingChampionship;
    
    [SetUp]
    public void Setup()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var options = new DbContextOptionsBuilder<CosmosDbContext>()
            .UseCosmos(_configuration.GetConnectionString("CosmosConnection"),
                databaseName: "FootballWebAppTests")
            .Options;

        _context = new CosmosDbContext(options);
        _context.Database.EnsureCreatedAsync().GetAwaiter().GetResult();

        var config = new MapperConfiguration(cfg => 
        { 
            cfg.AddProfile<ChampionshipMap>();
            cfg.AddProfile<PaginationMap>();
        });
        var mapper = config.CreateMapper();
        
        var championshipRepository = new ChampionshipRepository(mapper, _context);
        var championshipOrchestrator = new ChampionshipOrchestrator(championshipRepository);
        _championshipsController = new ChampionshipsController(mapper, championshipOrchestrator);
        
        var championships = new List<ChampionshipDao>
        {
            new ChampionshipDao
            {
                Id = Guid.NewGuid(),
                Name = "Premier League",
                DateOfCreation = new DateTime(2023, 1, 10),
                TeamAPoints = 10,
                TeamBPoints = 8,
                TeamCPoints = 6
            }
        };

        _context.Championships.AddRangeAsync(championships).GetAwaiter().GetResult();
        _context.SaveChangesAsync().GetAwaiter().GetResult();
        
        _existingChampionship = _context.Championships.FirstAsync().GetAwaiter().GetResult();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeletedAsync().GetAwaiter().GetResult();
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllChampionships()
    {
        var result = await _championshipsController.GetAllAsync(new Pagination
        {
            page = 1,
            pageSize = 10
        });

        var okResult = result as OkObjectResult;
        var championships = okResult?.Value as IEnumerable<ChampionshipDto>;
        Assert.That(championships?.Count(), Is.EqualTo(1));
    }
    
    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectChampionship()
    {
        var result = await _championshipsController.GetByIdAsync(_existingChampionship.Id);
        var okResult = result as OkObjectResult;
        var championship = okResult?.Value as ChampionshipDto;

        Assert.Multiple(() =>
        {
            Assert.That(championship, Is.Not.Null);
            Assert.That(championship?.Id, Is.EqualTo(_existingChampionship.Id));
            Assert.That(championship?.Name, Is.EqualTo(_existingChampionship.Name));
        });
    }
    
    [Test]
    public async Task CreateAsync_ShouldAddNewChampionship()
    {
        var newChampionship = new CreateChampionship
        {
            Name = "Serie A",
            DateOfCreation = DateTime.UtcNow,
            TeamAPoints = 10,
            TeamBPoints = 8,
            TeamCPoints = 6
        };

        var result = await _championshipsController.CreateAsync(newChampionship);

        var okResult = result as OkObjectResult;
        var createdChampionship = okResult?.Value as ChampionshipDto;
        Assert.That(createdChampionship?.Name, Is.EqualTo(newChampionship.Name));
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyChampionshipFields()
    {
        var updateModel = new CreateChampionship
        {
            Name = "Premier League",
            DateOfCreation = DateTime.UtcNow,
            TeamAPoints = 15,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        var updateResult = await _championshipsController.UpdateAsync(_existingChampionship.Id, updateModel);

        var okResult = updateResult as OkObjectResult;
        var updatedChampionship = okResult?.Value as ChampionshipDto;
        Assert.Multiple(() =>
        {
            Assert.That(updatedChampionship?.TeamAPoints, Is.EqualTo(updateModel.TeamAPoints));
            Assert.That(updatedChampionship?.TeamBPoints, Is.EqualTo(updateModel.TeamBPoints));
            Assert.That(updatedChampionship?.TeamCPoints, Is.EqualTo(updateModel.TeamCPoints));
        });
    }

    [Test]
    public async Task DeleteAsync_ShouldMarkChampionshipAsDeleted()
    {
        await _championshipsController.DeleteAsync(_existingChampionship.Id);

        Assert.ThrowsAsync<ChampionshipNotFoundException>(() => _championshipsController.GetByIdAsync(_existingChampionship.Id));
    }
} 