using AutoMapper;
using FootballWebApp.Data;
using FootballWebApp.Data.User;
using FootballWebApp.Model.Common;
using FootballWebApp.Model.User;
using FootballWebApp.Orchestrators.User;
using FootballWebApp.User;
using FootballWebApp.User.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FootballWebApp.IntegrationTests;

public class UserControllerTests
{
    private UsersController _usersController;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .EnableSensitiveDataLogging()
            .Options;

        var context = new SqlDbContext(options);
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<UserMap>(); cfg.AddProfile<PaginationMap>();});
        var mapper = config.CreateMapper();
        var userRepository = new UserRepository(context, mapper);
        var userOrchestrator = new UserOrchestrator(userRepository);
        _usersController = new UsersController(userOrchestrator, mapper);

        context.Users.AddRange(
            new UserDao
            {
                Id = 1,
                Nickname = "player_one",
                DateOfRegistration = new DateTime(2023, 1, 10),
                Email = "player1@example.com",
                Password = "Password123!"
            },
            new UserDao
            {
                Id = 2,
                Nickname = "pro_gamer",
                DateOfRegistration = new DateTime(2023, 3, 5),
                Email = "gamer2@example.com",
                Password = "SecurePass456!"
            },
            new UserDao
            {
                Id = 3,
                Nickname = "rookie_star",
                DateOfRegistration = new DateTime(2024, 2, 20),
                Email = "rookie3@example.com",
                Password = "Rookie789@"
            });
        
        context.SaveChanges();

       
    }
    [Test]
    public async Task Get_ShouldReturnAllUsers()
    {
        var result = await _usersController.GetAsync(new Pagination
        {
            page = 1,
            pageSize = 10
        });

        var okResult = result as OkObjectResult;
        var users = okResult.Value as IEnumerable<UserDto>;
        
        Assert.That(users.Count(), Is.EqualTo(3));
    }

    [Test]
    public async Task GetById_ShouldReturnCorrectUser()
    {
        var result = await _usersController.GetByIdAsync(1);

        var okResult = result as OkObjectResult;
    
        var user = okResult!.Value as UserDto;
        Assert.That(user!.Nickname, Is.EqualTo("player_one"));
    }

    [Test]
    public async Task CreateAsync_ShouldAddNewUser()
    {
        var newUser = new PostUser()
        {
            Nickname = "Modrik",
            Email = "oleg123@gmail.com",
            Password = "awqasdasdasdas"
        };

        var result = await _usersController.PostAsync(newUser);

        var okResult = result as OkObjectResult;

        var createdUser = okResult!.Value as UserDto;
        Assert.That(createdUser!.Nickname, Is.EqualTo(newUser.Nickname));
    }
    
    [Test]
    public async Task UpdateAsync_ShouldModifyUserFields()
    {
        var updateModel = new PostUser()
        {
            Nickname = "updated_nickname",
            Email = "updated_email@example.com",
            Password = "updated_password"
        };
        
        await _usersController.UpdateAsync(1, updateModel);
        var result = await _usersController.GetByIdAsync(1);
        
        var okResult = result as OkObjectResult;
        var updatedUser = okResult!.Value as UserDto;
        Assert.That(updatedUser.Email, Is.EqualTo(updateModel.Email));
    }

    [Test]
    public async Task DeleteAsync_ShouldMarkUserAsDeleted()
    {
        await _usersController.DeleteAsync(1);
        Assert.ThrowsAsync<UserNotFoundException>(() => _usersController.GetByIdAsync(1));
    }
}