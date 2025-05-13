#region

using Business.Models;
using Business.Services;
using Data.Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

#endregion

namespace FootballTournamentApp.UnitTests;

[TestFixture]
public class UserServiceTests
{
    private UserService _userService;
    private StoreDbContext _context;
    private Mock<IConfiguration> _configuration;

    [SetUp]
    public void Setup()
    {
        _configuration = new Mock<IConfiguration>();
        _configuration.Setup(c => c["Jwt:Key"]).Returns("12342131231241234123121132123123123");
        _configuration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configuration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .EnableSensitiveDataLogging()
            .Options;

        _context = new StoreDbContext(options);

        _context.Users.AddRange(
            new User
            {
                Id = 1,
                Nickname = "player_one",
                DateOfRegistration = new DateTime(2023, 1, 10),
                Email = "player1@example.com",
                Password = "Password123!"
            },
            new User
            {
                Id = 2,
                Nickname = "pro_gamer",
                DateOfRegistration = new DateTime(2023, 3, 5),
                Email = "gamer2@example.com",
                Password = "SecurePass456!"
            },
            new User
            {
                Id = 3,
                Nickname = "rookie_star",
                DateOfRegistration = new DateTime(2024, 2, 20),
                Email = "rookie3@example.com",
                Password = "Rookie789@"
            });
        _context.SaveChanges();
        _userService = new UserService(_context, _configuration.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetAllUsers_ShouldReturnAllUsers()
    {
        var result = await _userService.GetAllUsers();

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Any(u => u.Nickname == "player_one"));
        Assert.That(result.Any(u => u.Nickname == "pro_gamer"));
        Assert.That(result.Any(u => u.Nickname == "rookie_star"));
    }

    [Test]
    public async Task GetUserById_ShouldReturnCorrectUser()
    {
        var result = await _userService.GetUserById(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Nickname, Is.EqualTo("player_one"));
    }

    [Test]
    public void GetUserById_ShouldReturnNullIfUserNotFound()
    {
        var ex = Assert.ThrowsAsync<Exception>((async () => await _userService.GetUserById(999)));
        Assert.That(ex.Message, Is.EqualTo("User with id 999 not found"));
    }

    [Test]
    public async Task Register_ShouldAddNewUser()
    {
        var newUser = new UserRegisterModel
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            NickName = "new_user"
        };

        await _userService.Register(newUser);
        var result = await _userService.GetAllUsers();

        Assert.That(result.Count, Is.EqualTo(4));
        Assert.That(result.Any(u => u.Email == "newuser@example.com"));
    }

    [Test]
    public void Register_ShouldThrowArgumentNullException_WhenUserModelIsNull()
    {
        UserRegisterModel newUser = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () => await _userService.Register(newUser));
    }

    [Test]
    public void Register_ShouldThrowException_WhenEmailPasswordOrNickNameIsEmpty()
    {
        var newUser = new UserRegisterModel
        {
            Email = "   ",
            Password = "",
            NickName = null
        };

        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await _userService.Register(newUser));
        Assert.That(ex.Message, Is.EqualTo("Not all data fields are filled"));
    }

    [Test]
    public async Task UpdateUser_UserExists_UpdatesUser()
    {
        var updatedData = new UpdateUserModel
        {
            Nickname = "NewNickname",
            Email = "newemail@example.com"
        };

        await _userService.UpdateUser(1, updatedData);

        var user = await _context.Users.FindAsync(1);
        Assert.That(user, Is.Not.Null);
        Assert.That(user.Nickname, Is.EqualTo("NewNickname"));
        Assert.That(user.Email, Is.EqualTo("newemail@example.com"));
    }

    [Test]
    public void UpdateUser_UserDoesNotExist_ThrowsException()
    {
        var updatedData = new UpdateUserModel
        {
            Nickname = "NewNickname",
            Email = "newemail@example.com"
        };

        var ex = Assert.ThrowsAsync<Exception>(async () => await _userService.UpdateUser(99, updatedData));
        Assert.That(ex.Message, Is.EqualTo($"User with id {99} not found"));
    }

    [Test]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        await _userService.DeleteUser(1);
        var result = await _context.Users.FindAsync(1);

        Assert.That(result.Deleted, Is.EqualTo(true));
    }
}