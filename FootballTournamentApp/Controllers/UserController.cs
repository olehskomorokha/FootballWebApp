#region

using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;

#endregion

namespace FootballTournamentApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetUsers(int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("Page and page size must be greater than 0.");
        }

        var users = await _userService.GetAllUsers(page, pageSize);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<User> GetUserById(int id)
    {
        return await _userService.GetUserById(id);
    }

    [HttpPost]
    public async Task<ActionResult> Register(UserRegisterModel userRegisterModel)
    {
        await _userService.Register(userRegisterModel);
        return Ok(userRegisterModel);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(UserLoginModel userLoginModel)
    {
        var result = await _userService.Login(userLoginModel);
        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(int id, UpdateUserModel updatedUser)
    {
        await _userService.UpdateUser(id, updatedUser);
        return Ok("User updated successfully");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUser(id);
        return Ok("User deleted successfully");
    }
}