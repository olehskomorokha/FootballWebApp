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
        // Arrange
        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);

        // Act
        var result = await _championshipOrchestrator.GetAllAsync(new PaginationDto
        {
            page = 1,
            pageSize = 10
        });

        // Assert
        Assert.That(result.Count, Is.EqualTo(3));
        _mockChampionshipRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnCorrectChampionship()
    {
        // Arrange
        var expectedChampionship = _testChampionships[0];
        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(expectedChampionship.Id))
            .ReturnsAsync(expectedChampionship);

        // Act
        var result = await _championshipOrchestrator.GetByIdAsync(expectedChampionship.Id);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Premier League"));
        _mockChampionshipRepository.Verify(repo => repo.GetByIdAsync(expectedChampionship.Id), Times.Once);
    }

    [Test]
    public void GetByIdAsync_ShouldThrowExceptionIfChampionshipNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(nonExistentId))
            .ReturnsAsync((ChampionshipDto?)null);

        // Act & Assert
        Assert.ThrowsAsync<ChampionshipNotFoundException>(async () => 
            await _championshipOrchestrator.GetByIdAsync(nonExistentId));
        _mockChampionshipRepository.Verify(repo => repo.GetByIdAsync(nonExistentId), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ShouldAddNewChampionship()
    {
        // Arrange
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

        // Act
        await _championshipOrchestrator.CreateAsync(newChampionship);

        // Assert
        _mockChampionshipRepository.Verify(repo => repo.CreateAsync(It.IsAny<ChampionshipDto>()), Times.Once);
    }

    [Test]
    public void CreateAsync_ShouldNotAddExistingChampionship_ThrowException()
    {
        // Arrange
        var newChampionship = new ChampionshipDto
        {
            Name = "Premier League",
            TeamAPoints = 10,
            TeamBPoints = 8,
            TeamCPoints = 6
        };

        _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testChampionships);

        // Act & Assert
        Assert.ThrowsAsync<ChampionshipAlreadyExistsException>(async () => 
            await _championshipOrchestrator.CreateAsync(newChampionship));
        _mockChampionshipRepository.Verify(repo => repo.CreateAsync(It.IsAny<ChampionshipDto>()), Times.Never);
    }

    // [Test]
    // public async Task UpdateChampionship_ShouldUpdateSuccessfully()
    // {
    //     // Arrange
    //     var existingChampionship = _testChampionships[0];
    //     var updatedData = new ChampionshipDto
    //     {
    //         Id = existingChampionship.Id,
    //         Name = "Updated Premier League",
    //         TeamAPoints = 15,
    //         TeamBPoints = 12,
    //         TeamCPoints = 9
    //     };
    //
    //     _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
    //         .ReturnsAsync(existingChampionship);
    //     _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
    //         .ReturnsAsync(_testChampionships);
    //
    //     var result = await _championshipOrchestrator.UpdateAsync(updatedData);
    //
    //     // Assert
    //     Assert.That(result.Name, Is.EqualTo(updatedData.Name));
    //     _mockChampionshipRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ChampionshipDto>()), Times.Once);
    // }
    //
    // [Test]
    // public void UpdateChampionship_ChampionshipDoesNotExist_ThrowsException()
    // {
    //     // Arrange
    //     var updatedData = new ChampionshipDto
    //     {
    //         Id = Guid.NewGuid(),
    //         Name = "Non-existent Championship",
    //         TeamAPoints = 15,
    //         TeamBPoints = 12,
    //         TeamCPoints = 9
    //     };
    //
    //     _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(updatedData.Id))
    //         .ReturnsAsync((ChampionshipDto?)null);
    //
    //     // Act & Assert
    //     Assert.ThrowsAsync<ChampionshipNotFoundException>(async () => 
    //         await _championshipOrchestrator.UpdateAsync(updatedData));
    //     _mockChampionshipRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ChampionshipDto>()), Times.Never);
    // }
    //
    // [Test]
    // public async Task UpdateChampionship_WithNegativePoints_ThrowsArgumentException()
    // {
    //     // Arrange
    //     var existingChampionship = _testChampionships[0];
    //     var updatedData = new ChampionshipDto
    //     {
    //         Id = existingChampionship.Id,
    //         Name = "Updated Premier League",
    //         TeamAPoints = -5,  // Negative points
    //         TeamBPoints = 12,
    //         TeamCPoints = 9
    //     };
    //
    //     _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
    //         .ReturnsAsync(existingChampionship);
    //
    //     // Act & Assert
    //     var ex = Assert.ThrowsAsync<ArgumentException>(async () => 
    //         await _championshipOrchestrator.UpdateAsync(updatedData));
    //     Assert.That(ex.Message, Does.Contain("Points cannot be negative"));
    // }
    //
    // [Test]
    // public async Task UpdateChampionship_WithDuplicateName_ThrowsException()
    // {
    //     // Arrange
    //     var existingChampionship = _testChampionships[0];
    //     var updatedData = new ChampionshipDto
    //     {
    //         Id = existingChampionship.Id,
    //         Name = "La Liga", // Name of another existing championship
    //         TeamAPoints = 15,
    //         TeamBPoints = 12,
    //         TeamCPoints = 9
    //     };
    //
    //     _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
    //         .ReturnsAsync(existingChampionship);
    //     _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
    //         .ReturnsAsync(_testChampionships);
    //
    //     // Act & Assert
    //     Assert.ThrowsAsync<ChampionshipAlreadyExistsException>(async () => 
    //         await _championshipOrchestrator.UpdateAsync(updatedData));
    // }
    //
    // [Test]
    // public async Task UpdateChampionship_WithSameData_ShouldNotCallUpdate()
    // {
    //     // Arrange
    //     var existingChampionship = _testChampionships[0];
    //     var updatedData = new ChampionshipDto
    //     {
    //         Id = existingChampionship.Id,
    //         Name = existingChampionship.Name,
    //         TeamAPoints = existingChampionship.TeamAPoints,
    //         TeamBPoints = existingChampionship.TeamBPoints,
    //         TeamCPoints = existingChampionship.TeamCPoints,
    //         DateOfCreation = existingChampionship.DateOfCreation
    //     };
    //
    //     _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
    //         .ReturnsAsync(existingChampionship);
    //
    //     // Act
    //     await _championshipOrchestrator.UpdateAsync(updatedData);
    //
    //     // Assert
    //     _mockChampionshipRepository.Verify(repo => repo.UpdateAsync(It.IsAny<ChampionshipDto>()), Times.Never);
    // }
    //
    // [Test]
    // public async Task UpdateChampionship_PartialUpdate_ShouldOnlyUpdateProvidedFields()
    // {
    //     // Arrange
    //     var existingChampionship = _testChampionships[0];
    //     var partialUpdate = new ChampionshipDto
    //     {
    //         Id = existingChampionship.Id,
    //         Name = "Updated Name",
    //         TeamAPoints = existingChampionship.TeamAPoints,  // Keep existing points
    //         TeamBPoints = existingChampionship.TeamBPoints,
    //         TeamCPoints = existingChampionship.TeamCPoints,
    //         DateOfCreation = existingChampionship.DateOfCreation
    //     };
    //
    //     _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
    //         .ReturnsAsync(existingChampionship);
    //     _mockChampionshipRepository.Setup(repo => repo.GetAllAsync())
    //         .ReturnsAsync(_testChampionships);
    //     _mockChampionshipRepository.Setup(repo => repo.UpdateAsync(It.IsAny<ChampionshipDto>()))
    //         .ReturnsAsync(partialUpdate);
    //
    //     // Act
    //     var result = await _championshipOrchestrator.UpdateAsync(partialUpdate);
    //
    //     // Assert
    //     Assert.Multiple(() =>
    //     {
    //         Assert.That(result.Name, Is.EqualTo("Updated Name"));
    //         Assert.That(result.TeamAPoints, Is.EqualTo(existingChampionship.TeamAPoints));
    //         Assert.That(result.TeamBPoints, Is.EqualTo(existingChampionship.TeamBPoints));
    //         Assert.That(result.TeamCPoints, Is.EqualTo(existingChampionship.TeamCPoints));
    //     });
    // }

    [Test]
    public async Task DeleteChampionship_ShouldRemoveChampionship()
    {
        // Arrange
        var existingChampionship = _testChampionships[0];
        _mockChampionshipRepository.Setup(repo => repo.GetByIdAsync(existingChampionship.Id))
            .ReturnsAsync(existingChampionship);
        _mockChampionshipRepository.Setup(repo => repo.DeleteAsync(existingChampionship.Id))
            .ReturnsAsync(existingChampionship.Id);

        // Act
        await _championshipOrchestrator.DeleteAsync(existingChampionship.Id);

        // Assert
        _mockChampionshipRepository.Verify(repo => repo.DeleteAsync(existingChampionship.Id), Times.Once);
    }
} 