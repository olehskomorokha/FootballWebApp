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
    
    
}