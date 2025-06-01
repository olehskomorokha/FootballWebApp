using FootballWebApp.Model.Championship;
using FootballWebApp.Model.Common;
using FootballWebApp.Model.User;
using FootballWebApp.Model.UserChampionships;
using FootballWebApp.Orchestrator.Championship;
using FootballWebApp.Orchestrator.UserChampionship;
using FootballWebApp.Orchestrators.User;
using FootballWebApp.Platform.BlobStorage;
using Moq;

namespace FootballWebApp.UnitTests;

[TestFixture]
public class UserChampionshipOrchestratorTests
{
    private UserChampionshipOrchestrator _orchestrator;
    private Mock<IBlobStorage> _blobStorageMock;
    private Mock<IUserOrchestrator> _userOrchestratorMock;
    private Mock<IChampionshipOrchestrator> _championshipOrchestratorMock;
    private Guid _testChampionshipId;
    private int _testUserId;
    private UserDto _testUser;
    private ChampionshipDto _testChampionship;

    [SetUp]
    public void Setup()
    {
        _testChampionshipId = Guid.NewGuid();
        _testUserId = 1;
        
        _testUser = new UserDto 
        { 
            Id = _testUserId, 
            Nickname = "TestUser",
            Email = "test@example.com"
        };
        
        _testChampionship = new ChampionshipDto
        {
            Id = _testChampionshipId,
            Name = "Test Championship"
        };

        _blobStorageMock = new Mock<IBlobStorage>();
        _userOrchestratorMock = new Mock<IUserOrchestrator>();
        _championshipOrchestratorMock = new Mock<IChampionshipOrchestrator>();

        _userOrchestratorMock.Setup(x => x.GetUserByIdAsync(_testUserId))
            .ReturnsAsync(_testUser);
        
        _championshipOrchestratorMock.Setup(x => x.GetByIdAsync(_testChampionshipId))
            .ReturnsAsync(_testChampionship);

        _orchestrator = new UserChampionshipOrchestrator(
            _userOrchestratorMock.Object,
            _championshipOrchestratorMock.Object,
            _blobStorageMock.Object
        );
    }

    [Test]
    public async Task CreateAsync_WhenUserAndChampionshipExist_ShouldCreateFile()
    {
        _blobStorageMock.Setup(x => x.ContainsFileByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _orchestrator.CreateAsync(_testChampionshipId, _testUserId);

        Assert.Multiple(() =>
        {
            Assert.That(result.ChampionshipId, Is.EqualTo(_testChampionshipId));
            Assert.That(result.UserId, Is.EqualTo(_testUserId));
        });
        
        _blobStorageMock.Verify(x => x.CreateFileAsync(
            It.Is<string>(s => s == $"{_testChampionshipId:N}_{_testUserId}")), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WhenFileExists_ShouldNotCreateNewFile()
    {
        _blobStorageMock.Setup(x => x.ContainsFileByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _orchestrator.CreateAsync(_testChampionshipId, _testUserId);

        _blobStorageMock.Verify(x => x.CreateFileAsync(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CreateAsync_WhenUserDoesNotExist_ShouldThrowException()
    {
        _userOrchestratorMock.Setup(x => x.GetUserByIdAsync(_testUserId))
            .ThrowsAsync(new UserNotFoundException());

        Assert.ThrowsAsync<UserNotFoundException>(() => 
            _orchestrator.CreateAsync(_testChampionshipId, _testUserId));
    }

    [Test]
    public void CreateAsync_WhenChampionshipDoesNotExist_ShouldThrowException()
    {
        _championshipOrchestratorMock.Setup(x => x.GetByIdAsync(_testChampionshipId))
            .ThrowsAsync(new ChampionshipNotFoundException());

        Assert.ThrowsAsync<ChampionshipNotFoundException>(() => 
            _orchestrator.CreateAsync(_testChampionshipId, _testUserId));
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnUsersList()
    {
        var expectedUsers = new List<int> { 1, 2, 3 };
        _blobStorageMock.Setup(x => x.GetAllFilesByNameAsync(_testChampionshipId))
            .ReturnsAsync(expectedUsers);

        var result = await _orchestrator.GetAllAsync(_testChampionshipId);

        Assert.That(result, Is.EqualTo(expectedUsers));
        _blobStorageMock.Verify(x => x.GetAllFilesByNameAsync(_testChampionshipId), Times.Once);
    }

    [Test]
    public async Task GetUserChampionshipAsync_ShouldReturnFileContent()
    {
        var expectedStream = new MemoryStream();
        _blobStorageMock.Setup(x => x.GetFileByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedStream);

        var result = await _orchestrator.GetUserChampionshipAsync(_testChampionshipId, _testUserId);

        Assert.That(result, Is.EqualTo(expectedStream));
        _blobStorageMock.Verify(x => x.GetFileByNameAsync(
            It.Is<string>(s => s == $"{_testChampionshipId:N}_{_testUserId}")), Times.Once);
    }

    [Test]
    public async Task DeleteUserChampionshipAsync_ShouldDeleteFile()
    {
        await _orchestrator.DeleteUserChampionshipAsync(_testChampionshipId, _testUserId);

        _blobStorageMock.Verify(x => x.DeleteFileAsync(
            It.Is<string>(s => s == $"{_testChampionshipId:N}_{_testUserId}")), Times.Once);
    }

    [Test]
    public async Task UpdateUserChampionshipAsync_ShouldUpdateFile()
    {
        var content = new MemoryStream();

        await _orchestrator.UpdateUserChampionshipAsync(_testChampionshipId, _testUserId, content);

        _blobStorageMock.Verify(x => x.UpdateFileAsync(
            It.Is<string>(s => s == $"{_testChampionshipId:N}_{_testUserId}"),
            It.Is<Stream>(stream => stream == content)), Times.Once);
    }
} 