using FootballWebApp.Model.Common;
using FootballWebApp.Model.User;
using FootballWebApp.Orchestrators.User;
using Moq;

namespace FootballWebApp.UnitTests;

[TestFixture]
public class UserOrchestratorTests
{
    private UserOrchestrator _userOrchestrator = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private List<UserDto> _testUsers = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userOrchestrator = new UserOrchestrator(_mockUserRepository.Object);

        _testUsers = new List<UserDto>
        {
            new UserDto
            {
                Id = 1,
                Nickname = "player_one",
                DateOfRegistration = new DateTime(2023, 1, 10),
                Email = "player1@example.com",
                Password = "Password123!"
            },
            new UserDto
            {
                Id = 2,
                Nickname = "pro_gamer",
                DateOfRegistration = new DateTime(2023, 3, 5),
                Email = "gamer2@example.com",
                Password = "SecurePass456!"
            },
            new UserDto
            {
                Id = 3,
                Nickname = "rookie_star",
                DateOfRegistration = new DateTime(2024, 2, 20),
                Email = "rookie3@example.com",
                Password = "Rookie789@"
            }
        };
    }

    [Test]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(_testUsers);

        var result = await _userOrchestrator.GetAllUsersAsync(new PaginationDto
        {
            page = 1,
            pageSize = 10
        });

        Assert.That(result.Count, Is.EqualTo(3));
        _mockUserRepository.Verify(repo => repo.GetAllUsersAsync(), Times.Once);
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnCorrectUser()
    {
        var expectedUser = _testUsers[0];
        _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
            .ReturnsAsync(expectedUser);

        var result = await _userOrchestrator.GetUserByIdAsync(1);

        Assert.That(result.Nickname, Is.EqualTo("player_one"));
        _mockUserRepository.Verify(repo => repo.GetUserByIdAsync(1), Times.Once);
    }

    [Test]
    public void GetUserByIdAsync_ShouldReturnNullIfUserNotFound()
    {
        _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(999))
            .ReturnsAsync((UserDto?)null);

        Assert.ThrowsAsync<UserNotFoundException>(async () => await _userOrchestrator.GetUserByIdAsync(999));
        _mockUserRepository.Verify(repo => repo.GetUserByIdAsync(999), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ShouldAddNewUser()
    {
        var newUser = new UserDto
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            Nickname = "new_user"
        };

        _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(_testUsers);
        _mockUserRepository.Setup(repo => repo.CreateAsync(It.IsAny<UserDto>()))
            .ReturnsAsync((UserDto u) => u);

        await _userOrchestrator.CreateAsync(newUser);

        _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<UserDto>()), Times.Once);
    }
    
    [Test]
    public void CreateAsync_ShouldNotAddAlreadyExistUser_ThrowException()
    {
        var newUser = new UserDto
        {
            Email = "gamer2@example.com",
            Password = "Password123!",
            Nickname = "new_user"
        };

        _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(_testUsers);
        Assert.ThrowsAsync<UserAlreadyExistsException>(async () => await _userOrchestrator.CreateAsync(newUser));
        _mockUserRepository.Verify(repo => repo.CreateAsync(It.IsAny<UserDto>()), Times.Never);
    }

    [Test]
    public async Task UpdateUser_ShouldUpdateEmail_UpdateUserSuccessfully()
    {
        var existingUser = _testUsers[0];
        var updatedData = new UserDto
        {
            Id = 1,
            Nickname = "NewNickname",
            Email = "newemail@example.com"
        };

        _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(repo => repo.GetAllUsersAsync())
            .ReturnsAsync(_testUsers);
        _mockUserRepository.Setup(repo => repo.UpdateUserAsync(1, It.IsAny<UserDto>()))
            .ReturnsAsync(updatedData);
        var result = await _userOrchestrator.UpdateUserAsync(1, updatedData);

        Assert.That(result.Email, Is.EqualTo(updatedData.Email));
        _mockUserRepository.Verify(repo => repo.UpdateUserAsync(1, It.IsAny<UserDto>()), Times.Once);
    }

    [Test]
    public void UpdateUser_UserDoesNotExist_ThrowsException()
    {
        var updatedData = new UserDto
        {
            Nickname = "NewNickname",
            Email = "newemail@example.com",
        };

        _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(99))
            .ReturnsAsync((UserDto?)null);

        Assert.ThrowsAsync<UserNotFoundException>(async () => await _userOrchestrator.UpdateUserAsync(99, updatedData));
        _mockUserRepository.Verify(repo => repo.UpdateUserAsync(It.IsAny<int>(), It.IsAny<UserDto>()), Times.Never);
    }

    [Test]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        var existingUser = _testUsers[0];
        _mockUserRepository.Setup(repo => repo.GetUserByIdAsync(1))
            .ReturnsAsync(existingUser);
        _mockUserRepository.Setup(repo => repo.DeleteUserAsync(1))
            .ReturnsAsync(1);

        await _userOrchestrator.DeleteUserAsync(1);

        _mockUserRepository.Verify(repo => repo.DeleteUserAsync(1), Times.Once);
    }
}