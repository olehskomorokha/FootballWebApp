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
            },
            new ChampionshipDao
            {
                Id = Guid.NewGuid(),
                Name = "La Liga",
                DateOfCreation = new DateTime(2023, 3, 5),
                TeamAPoints = 12,
                TeamBPoints = 9,
                TeamCPoints = 7
            },
            new ChampionshipDao
            {
                Id = Guid.NewGuid(),
                Name = "Bundesliga",
                DateOfCreation = new DateTime(2024, 2, 20),
                TeamAPoints = 15,
                TeamBPoints = 11,
                TeamCPoints = 8
            }
        };

        _context.Championships.AddRangeAsync(championships).GetAwaiter().GetResult();
        _context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeletedAsync().GetAwaiter().GetResult();
        _context.Dispose();
    }

    [Test]
    public async Task Get_ShouldReturnAllChampionships()
    {
        var result = await _championshipsController.GetAllAsync(new Pagination
        {
            page = 1,
            pageSize = 10
        });

        var okResult = result as OkObjectResult;
        var championships = okResult?.Value as IEnumerable<ChampionshipDto>;
        Assert.That(championships?.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetById_ShouldReturnCorrectChampionship()
    {
        var championship = (await _context.Championships.FirstAsync()).Id;

        var result = await _championshipsController.GetByIdAsync(championship);

        var okResult = result as OkObjectResult;
        var returnedChampionship = okResult?.Value as ChampionshipDto;
        Assert.That(returnedChampionship?.Name, Is.EqualTo("Premier League"));
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
        var championship = await _context.Championships.FirstAsync();
        var updateModel = new CreateChampionship
        {
            Name = "Updated Premier League",
            DateOfCreation = DateTime.UtcNow,
            TeamAPoints = 15,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        await _championshipsController.UpdateAsync(championship.Id, updateModel);
        var result = await _championshipsController.GetByIdAsync(championship.Id);

        var okResult = result as OkObjectResult;
        var updatedChampionship = okResult?.Value as ChampionshipDto;
        Assert.That(updatedChampionship?.Name, Is.EqualTo(updateModel.Name));
    }

    [Test]
    public async Task UpdateAsync_WithDuplicateName_ShouldReturnConflict()
    {
        // Arrange
        var championship = await _context.Championships.FirstAsync();
        var existingName = (await _context.Championships.Skip(1).FirstAsync()).Name;
        var updateModel = new CreateChampionship
        {
            Name = existingName,
            DateOfCreation = DateTime.UtcNow,
            TeamAPoints = 15,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        // Act
        var result = await _championshipsController.UpdateAsync(championship.Id, updateModel);

        // Assert
        Assert.That(result, Is.TypeOf<ConflictObjectResult>());
    }

    [Test]
    public async Task UpdateAsync_WithNegativePoints_ShouldReturnBadRequest()
    {
        // Arrange
        var championship = await _context.Championships.FirstAsync();
        var updateModel = new CreateChampionship
        {
            Name = "Valid Name",
            DateOfCreation = DateTime.UtcNow,
            TeamAPoints = -5,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        // Act
        var result = await _championshipsController.UpdateAsync(championship.Id, updateModel);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateModel = new CreateChampionship
        {
            Name = "Valid Name",
            DateOfCreation = DateTime.UtcNow,
            TeamAPoints = 15,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        // Act
        var result = await _championshipsController.UpdateAsync(nonExistentId, updateModel);

        // Assert
        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteAsync_ShouldMarkChampionshipAsDeleted()
    {
        var championship = await _context.Championships.FirstAsync();

        await _championshipsController.DeleteAsync(championship.Id);

        Assert.ThrowsAsync<ChampionshipNotFoundException>(() => 
            _championshipsController.GetByIdAsync(championship.Id));
    }
} 