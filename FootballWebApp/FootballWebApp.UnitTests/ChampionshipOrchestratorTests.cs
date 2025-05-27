using FootballWebApp.Model.Championship;
using FootballWebApp.Model.Common;
using FootballWebApp.Orchestrator.Championship;
using Moq;

namespace FootballWebApp.UnitTests;

[TestFixture]
public class ChampionshipOrchestratorTests
{
    private ChampionshipOrchestrator _championshipOrchestrator = null!;
    private Mock<IChampionshipRepository> _mockChampionshipRepository = null!;
    private List<ChampionshipDto> _testChampionships = null!;

    [SetUp]
    public void Setup()
    {
        _mockChampionshipRepository = new Mock<IChampionshipRepository>();
        _championshipOrchestrator = new ChampionshipOrchestrator(_mockChampionshipRepository.Object);

        _testChampionships = new List<ChampionshipDto>
        {
            new ChampionshipDto
            {
                Id = Guid.NewGuid(),
                Name = "Premier League",
                DateOfCreation = new DateTime(2023, 1, 10),
                TeamAPoints = 10,
                TeamBPoints = 8,
                TeamCPoints = 6
            },
            new ChampionshipDto
            {
                Id = Guid.NewGuid(),
                Name = "La Liga",
                DateOfCreation = new DateTime(2023, 3, 5),
                TeamAPoints = 12,
                TeamBPoints = 9,
                TeamCPoints = 7
            },
            new ChampionshipDto
            {
                Id = Guid.NewGuid(),
                Name = "Bundesliga",
                DateOfCreation = new DateTime(2024, 2, 20),
                TeamAPoints = 15,
                TeamBPoints = 11,
                TeamCPoints = 8
            }
        };
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllChampionships()
    {
        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);

        var result = await _championshipOrchestrator.GetAllAsync(new PaginationDto
        {
            page = 1,
            pageSize = 10
        });

        Assert.That(result.Count, Is.EqualTo(3));
        _mockChampionshipRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectChampionship()
    {
        var expectedChampionship = _testChampionships[0];
        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(expectedChampionship.Id))
            .ReturnsAsync(expectedChampionship);

        var result = await _championshipOrchestrator.GetByIdAsync(expectedChampionship.Id);

        Assert.That(result.Name, Is.EqualTo("Premier League"));
        _mockChampionshipRepository.Verify(repo => repo.GetByIdAsync(expectedChampionship.Id), Times.Once);
    }

    [Test]
    public void GetByIdAsync_ShouldThrowExceptionIfChampionshipNotFound()
    {
        var nonExistentId = Guid.NewGuid();
        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((ChampionshipDto?)null);

        Assert.ThrowsAsync<ChampionshipNotFoundException>(async () => 
            await _championshipOrchestrator.GetByIdAsync(nonExistentId));
        _mockChampionshipRepository.Verify(repo => repo.GetByIdAsync(nonExistentId), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ShouldAddNewChampionship()
    {
        var newChampionship = new ChampionshipDto
        {
            Name = "Serie A",
            TeamAPoints = 10,
            TeamBPoints = 8,
            TeamCPoints = 6
        };

        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);
        _mockChampionshipRepository.Setup(repo => repo.CreateAsync(It.IsAny<ChampionshipDto>()))
            .ReturnsAsync((ChampionshipDto c) => c);

        await _championshipOrchestrator.CreateAsync(newChampionship);

        _mockChampionshipRepository.Verify(repo => repo.CreateAsync(It.IsAny<ChampionshipDto>()), Times.Once);
    }

    [Test]
    public void CreateAsync_ShouldNotAddExistingChampionship_ThrowException()
    {
        var newChampionship = new ChampionshipDto
        {
            Name = "Premier League",
            TeamAPoints = 10,
            TeamBPoints = 8,
            TeamCPoints = 6
        };

        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);

        Assert.ThrowsAsync<ChampionshipAlreadyExistsException>(async () => 
            await _championshipOrchestrator.CreateAsync(newChampionship));
        _mockChampionshipRepository.Verify(repo => repo.CreateAsync(It.IsAny<ChampionshipDto>()), Times.Never);
    }

    [Test]
    public async Task UpdateChampionship_ShouldUpdateSuccessfully()
    {
        var existingChampionship = _testChampionships[0];
        var updatedData = new ChampionshipDto
        {
            Id = existingChampionship.Id,
            Name = "Updated Premier League",
            TeamAPoints = 15,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
            .ReturnsAsync(existingChampionship);
        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);
        _mockChampionshipRepository.Setup(repo => repo.UpdateAsync(existingChampionship.Id, It.IsAny<ChampionshipDto>()))
            .ReturnsAsync(updatedData);

        var result = await _championshipOrchestrator.UpdateAsync(existingChampionship.Id, updatedData);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(updatedData.Name));
            Assert.That(result.TeamAPoints, Is.EqualTo(updatedData.TeamAPoints));
            Assert.That(result.TeamBPoints, Is.EqualTo(updatedData.TeamBPoints));
            Assert.That(result.TeamCPoints, Is.EqualTo(updatedData.TeamCPoints));
        });
        _mockChampionshipRepository.Verify(repo => repo.UpdateAsync(existingChampionship.Id, It.IsAny<ChampionshipDto>()), Times.Once);
    }

    [Test]
    public void UpdateChampionship_ChampionshipDoesNotExist_ThrowsException()
    {
        var nonExistentId = Guid.NewGuid();
        var updatedData = new ChampionshipDto
        {
            Id = nonExistentId,
            Name = "Non-existent Championship",
            TeamAPoints = 15,
            TeamBPoints = 12,
            TeamCPoints = 9
        };

        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((ChampionshipDto?)null);

        Assert.ThrowsAsync<ChampionshipNotFoundException>(async () => 
            await _championshipOrchestrator.UpdateAsync(nonExistentId, updatedData));
        _mockChampionshipRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Guid>(), It.IsAny<ChampionshipDto>()), Times.Never);
    }

    [Test]
    public async Task UpdateChampionship_WithSameName_ShouldSucceed()
    {
        var existingChampionship = _testChampionships[0];
        var updatedData = new ChampionshipDto
        {
            Id = existingChampionship.Id,
            Name = existingChampionship.Name,
            TeamAPoints = 20,
            TeamBPoints = 15,
            TeamCPoints = 10
        };

        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
            .ReturnsAsync(existingChampionship);
        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);
        _mockChampionshipRepository.Setup(repo => repo.UpdateAsync(existingChampionship.Id, It.IsAny<ChampionshipDto>()))
            .ReturnsAsync(updatedData);

        var result = await _championshipOrchestrator.UpdateAsync(existingChampionship.Id, updatedData);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(existingChampionship.Name));
            Assert.That(result.TeamAPoints, Is.EqualTo(20));
            Assert.That(result.TeamBPoints, Is.EqualTo(15));
            Assert.That(result.TeamCPoints, Is.EqualTo(10));
        });
        _mockChampionshipRepository.Verify(repo => repo.UpdateAsync(existingChampionship.Id, It.IsAny<ChampionshipDto>()), Times.Once);
    }

    [Test]
    public async Task UpdateChampionship_PartialUpdate_ShouldOnlyUpdateProvidedFields()
    {
        var existingChampionship = _testChampionships[0];
        var updatedData = new ChampionshipDto
        {
            Id = existingChampionship.Id,
            Name = "Updated Name",
            TeamAPoints = existingChampionship.TeamAPoints,
            TeamBPoints = existingChampionship.TeamBPoints,
            TeamCPoints = existingChampionship.TeamCPoints,
            DateOfCreation = existingChampionship.DateOfCreation
        };

        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
            .ReturnsAsync(existingChampionship);
        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);
        _mockChampionshipRepository.Setup(repo => repo.UpdateAsync(existingChampionship.Id, It.IsAny<ChampionshipDto>()))
            .ReturnsAsync(updatedData);

        var result = await _championshipOrchestrator.UpdateAsync(existingChampionship.Id, updatedData);

        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo("Updated Name"));
            Assert.That(result.TeamAPoints, Is.EqualTo(existingChampionship.TeamAPoints));
            Assert.That(result.TeamBPoints, Is.EqualTo(existingChampionship.TeamBPoints));
            Assert.That(result.TeamCPoints, Is.EqualTo(existingChampionship.TeamCPoints));
            Assert.That(result.DateOfCreation, Is.EqualTo(existingChampionship.DateOfCreation));
        });
    }

    [Test]
    public async Task DeleteChampionship_ShouldRemoveChampionship()
    {
        var existingChampionship = _testChampionships[0];
        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
            .ReturnsAsync(existingChampionship);
        _mockChampionshipRepository.Setup(repo => repo.DeleteAsync(existingChampionship.Id))
            .ReturnsAsync(existingChampionship.Id);

        await _championshipOrchestrator.DeleteAsync(existingChampionship.Id);

        _mockChampionshipRepository.Verify(repo => repo.DeleteAsync(existingChampionship.Id), Times.Once);
    }
} 