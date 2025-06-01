using FootballWebApp.Model.Championship;
using FootballWebApp.Model.User;
using FootballWebApp.Model.UserChampionships;
using FootballWebApp.Orchestrator.Championship;
using FootballWebApp.Orchestrator.UserChampionship;
using FootballWebApp.Orchestrators.User;
using FootballWebApp.Platform.BlobStorage;
using FootballWebApp.UserChampionships;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootballWebApp.IntegrationTests;

[TestFixture]
public class UserChampionshipsControllerTests
{
    private UserChampionshipsController _controller;
    private Mock<IBlobStorage> _blobStorageMock;
    private Mock<IUserOrchestrator> _userOrchestratorMock;
    private Mock<IChampionshipOrchestrator> _championshipOrchestratorMock;
    private Guid _testChampionshipId;
    private int _testUserId;
    private UserChampionshipContent _testContent;

    [SetUp]
    public void Setup()
    {
        _testChampionshipId = Guid.NewGuid();
        _testUserId = 1;
        
        _testContent = new UserChampionshipContent
        {
            JoinDate = DateTime.UtcNow,
            Score = 100,
            Role = "Player",
            Notes = "Test notes",
            IsActive = true,
            Matches = new List<MatchParticipation>
            {
                new()
                {
                    MatchDate = DateTime.UtcNow,
                    GoalsScored = 2,
                    Assists = 1,
                    Position = "Forward"
                }
            }
        };

        _blobStorageMock = new Mock<IBlobStorage>();
        _userOrchestratorMock = new Mock<IUserOrchestrator>();
        _championshipOrchestratorMock = new Mock<IChampionshipOrchestrator>();

        _userOrchestratorMock.Setup(x => x.GetUserByIdAsync(_testUserId))
            .ReturnsAsync(new UserDto { Id = _testUserId, Nickname = "TestUser" });

        _championshipOrchestratorMock.Setup(x => x.GetByIdAsync(_testChampionshipId))
            .ReturnsAsync(new ChampionshipDto { Id = _testChampionshipId, Name = "Test Championship" });

        var orchestrator = new UserChampionshipOrchestrator(
            _userOrchestratorMock.Object,
            _championshipOrchestratorMock.Object,
            _blobStorageMock.Object
        );

        _controller = new UserChampionshipsController(orchestrator);
    }

    [Test]
    public async Task CreateAsync_ShouldCreateFileInBlobStorage()
    {
        _blobStorageMock.Setup(x => x.ContainsFileByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _controller.PostAsync(_testChampionshipId, _testUserId);

        _blobStorageMock.Verify(x => x.CreateFileAsync(It.Is<string>(s => 
            s == $"{_testChampionshipId:N}_{_testUserId}")), Times.Once);
        
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnUsersList()
    {
        var expectedUsers = new List<int> { 1, 2, 3 };
        _blobStorageMock.Setup(x => x.GetAllFilesByNameAsync(_testChampionshipId))
            .ReturnsAsync(expectedUsers);

        var result = await _controller.GetAllAsync(_testChampionshipId);

        var okResult = result as OkObjectResult;
        var users = okResult?.Value as List<int>;
        
        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.Not.Null);
            Assert.That(users, Is.EqualTo(expectedUsers));
        });
        
        _blobStorageMock.Verify(x => x.GetAllFilesByNameAsync(_testChampionshipId), Times.Once);
    }

    [Test]
    public async Task GetUserChampionshipAsync_ShouldReturnFileContent()
    {
        var testStream = new MemoryStream();
        _blobStorageMock.Setup(x => x.GetFileByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(testStream);

        var result = await _controller.GetUserChampionshipAsync(_testChampionshipId, _testUserId);

        var fileResult = result as FileStreamResult;
        Assert.Multiple(() =>
        {
            Assert.That(fileResult, Is.Not.Null);
            Assert.That(fileResult?.ContentType, Is.EqualTo("application/json"));
        });
        
        _blobStorageMock.Verify(x => x.GetFileByNameAsync(
            It.Is<string>(s => s == $"{_testChampionshipId:N}_{_testUserId}")), Times.Once);
    }

    [Test]
    public async Task UpdateUserChampionshipAsync_ShouldUpdateFileContent()
    {
        var result = await _controller.UpdateUserChampionshipAsync(_testChampionshipId, _testUserId, _testContent);

        _blobStorageMock.Verify(x => x.UpdateFileAsync(
            It.Is<string>(s => s == $"{_testChampionshipId:N}_{_testUserId}"),
            It.IsAny<Stream>()), Times.Once);
        
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task DeleteUserChampionshipAsync_ShouldDeleteFile()
    {
        var result = await _controller.DeleteUserChampionshipAsync(_testChampionshipId, _testUserId);

        _blobStorageMock.Verify(x => x.DeleteFileAsync(
            It.Is<string>(s => s == $"{_testChampionshipId:N}_{_testUserId}")), Times.Once);
        
        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }
} 