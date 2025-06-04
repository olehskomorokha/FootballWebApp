using AutoMapper;
using FootballWebApp.Data;
using FootballWebApp.Data.Championship;
using FootballWebApp.Model.Championship;
using FootballWebApp.Model.Common;
using FootballWebApp.Championship.Contract;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;

namespace FootballWebApp.IntegrationTests;

[TestFixture]
public class ChampionshipControllerTests : IAsyncDisposable
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private IServiceScope _scope = null!;
    private CosmosDbContext _context = null!;
    private ChampionshipDao _existingChampionship = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [SetUp]
    public async Task Setup()
    {
        _scope = _factory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<CosmosDbContext>();
        await _context.Database.EnsureCreatedAsync();

        _existingChampionship = new ChampionshipDao
        {
            Id = Guid.NewGuid(),
            Name = "Premier League",
            DateOfCreation = new DateTime(2023, 1, 10),
            TeamAPoints = 10,
            TeamBPoints = 8,
            TeamCPoints = 6,
            Deleted = false
        };

        await _context.Championships.AddAsync(_existingChampionship);
        await _context.SaveChangesAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_context != null)
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.DisposeAsync();
        }

        if (_scope != null)
        {
            _scope.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_context != null)
        {
            await _context.DisposeAsync();
        }

        if (_scope != null)
        {
            _scope.Dispose();
        }

        if (_client != null)
        {
            _client.Dispose();
        }

        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllChampionships()
    {
        var response = await _client.GetAsync("/api/v1/championships?page=1&pageSize=10");
        var content = await response.Content.ReadAsStringAsync();
        var championships = JsonSerializer.Deserialize<List<ChampionshipDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(championships, Is.Not.Null);
            Assert.That(championships?.Count, Is.EqualTo(1));
            Assert.That(championships?[0].Name, Is.EqualTo("Premier League"));
        });
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectChampionship()
    {
        var response = await _client.GetAsync($"/api/v1/championships/{_existingChampionship.Id}");
        var content = await response.Content.ReadAsStringAsync();
        var championship = JsonSerializer.Deserialize<ChampionshipDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
            TeamAPoints = 10,
            TeamBPoints = 8,
            TeamCPoints = 6
        };

        var json = JsonSerializer.Serialize(newChampionship);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/v1/championships", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdChampionship = JsonSerializer.Deserialize<ChampionshipDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(createdChampionship, Is.Not.Null);
            Assert.That(createdChampionship?.Name, Is.EqualTo(newChampionship.Name));
            Assert.That(createdChampionship?.TeamAPoints, Is.EqualTo(newChampionship.TeamAPoints));
        });
    }

    [Test]
    public async Task UpdateAsync_ShouldModifyChampionship()
    {
        var updateModel = new CreateChampionship
        {
            Name = "Updated Premier League",
            TeamAPoints = 15,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        var json = JsonSerializer.Serialize(updateModel);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/v1/championships/{_existingChampionship.Id}", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var updatedChampionship = JsonSerializer.Deserialize<ChampionshipDto>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(updatedChampionship, Is.Not.Null);
            Assert.That(updatedChampionship?.Name, Is.EqualTo(updateModel.Name));
            Assert.That(updatedChampionship?.TeamAPoints, Is.EqualTo(updateModel.TeamAPoints));
        });
    }

    [Test]
    public async Task DeleteAsync_ShouldMarkChampionshipAsDeleted()
    {
        var response = await _client.DeleteAsync($"/api/v1/championships/{_existingChampionship.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var getAllResponse = await _client.GetAsync("/api/v1/championships?page=1&pageSize=10");
        var content = await getAllResponse.Content.ReadAsStringAsync();
        var championships = JsonSerializer.Deserialize<List<ChampionshipDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.That(championships?.Count, Is.EqualTo(0));
    }
} 