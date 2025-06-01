using System.Text.Json;
using FootballWebApp.Model.UserChampionships;
using Microsoft.AspNetCore.Mvc;

namespace FootballWebApp.UserChampionships;

[ApiController]
[Route("api/v1/user-championships")]
public class UserChampionshipsController : ControllerBase
{
    private readonly IUserChampionshipOrchestrator _userChampionshipOrchestrator;
    public UserChampionshipsController(IUserChampionshipOrchestrator userChampionshipOrchestrator)
    {
        _userChampionshipOrchestrator = userChampionshipOrchestrator;
    }
    [HttpPost("{championshipId}/users/{userId}")]
    public async Task<IActionResult> PostAsync(Guid championshipId, int userId)
    {
        var model = await _userChampionshipOrchestrator.CreateAsync(championshipId, userId);
        return Ok(model);
    }

    [HttpGet("{championshipId}/users")]
    public async Task<IActionResult> GetAllAsync(Guid championshipId)
    {
        var users = await _userChampionshipOrchestrator.GetAllAsync(championshipId);
        return Ok(users);
    }

    [HttpGet("{championshipId}/users/{userId}")]
    public async Task<IActionResult> GetUserChampionshipAsync(Guid championshipId, int userId)
    {
        var content = await _userChampionshipOrchestrator.GetUserChampionshipAsync(championshipId, userId);
        return File(content, "application/json");
    }

    [HttpDelete("{championshipId}/users/{userId}")]
    public async Task<IActionResult> DeleteUserChampionshipAsync(Guid championshipId, int userId)
    {
        await _userChampionshipOrchestrator.DeleteUserChampionshipAsync(championshipId, userId);
        return NoContent();
    }

    [HttpPut("{championshipId}/users/{userId}")]
    public async Task<IActionResult> UpdateUserChampionshipAsync(Guid championshipId, int userId, [FromBody] UserChampionshipContent content)
    {
        var jsonContent = JsonSerializer.Serialize(content);
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent));
        
        await _userChampionshipOrchestrator.UpdateUserChampionshipAsync(championshipId, userId, stream);
        return NoContent();
    }
}