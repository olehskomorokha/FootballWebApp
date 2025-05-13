#region

using Business.Models;
using Business.Services;
using Data.Data;
using Data.Entities;
using FootballTournamentApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

#endregion

namespace FootballTournamentApp.UnitTests;

[TestFixture]
public class UserControllerTests
{
    private StoreDbContext _context;
    private UserController _userController;
    private IConfiguration _configuration;

    [SetUp]
    public void Setup()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<StoreDbContext>()
            .UseInMemoryDatabase("IntegrationTestDb", b => b.EnableNullChecks(false))
            .EnableSensitiveDataLogging()
            .Options;

        _context = new StoreDbContext(options);

        // Initialize PasswordHasher
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Jwt:Key", "12342131231241234123121132123123123"),
                new KeyValuePair<string, string>("Jwt:Issuer", "TestIssuer"),
                new KeyValuePair<string, string>("Jwt:Audience", "TestAudience")
            })
            .Build();


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
            }
        );
        _context.SaveChanges();
        var userService = new UserService(_context, _configuration);
        _userController = new UserController(userService);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetUsers_ShouldReturnAllUsers()
    {
        var result = await _userController.GetUsers();

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult!.Value, Is.InstanceOf<IEnumerable<User>>());

        var users = okResult.Value as IEnumerable<User>;
        Assert.That(users, Is.Not.Null);
        Assert.That(users.Count(), Is.EqualTo(2));
        Assert.That(users.Any(u => u.Nickname == "player_one"));
        Assert.That(users.Any(u => u.Nickname == "pro_gamer"));
    }

    [Test]
    public async Task GetUserById_ShouldReturnCorrectUser()
    {
        var result = await _userController.GetUserById(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Nickname, Is.EqualTo("player_one"));
    }

    [Test]
    public async Task Register_ShouldAddNewUser()
    {
        var newUser = new UserRegisterModel
        {
            NickName = "Modrik",
            Email = "oleg123@gmail.com",
            Password = "awqasdasdasdas"
        };
        await _userController.Register(newUser);

        var createdUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == "oleg123@gmail.com");

        Assert.That(createdUser, Is.Not.Null);
        Assert.That(createdUser.Nickname, Is.EqualTo(newUser.NickName));
    }
   
    [Test]
    public async Task UpdateUser_ShouldModifyUserFields()
    {
        var userId = 1;
        var updateModel = new UpdateUserModel
        {
            Nickname = "updated_nickname",
            Email = "updated_email@example.com"
        };

        await _userController.UpdateUser(userId, updateModel);

        var updatedUser = await _context.Users.FindAsync(userId);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser.Nickname, Is.EqualTo(updateModel.Nickname));
        Assert.That(updatedUser.Email, Is.EqualTo(updateModel.Email));
    }

    [Test]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        var userId = 1;

        await _userController.DeleteUser(userId);

        var deletedUser = _context.Users.Find(userId);
        Assert.That(deletedUser.Deleted, Is.EqualTo(true));
    }
}