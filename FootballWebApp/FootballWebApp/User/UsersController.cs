using AutoMapper;
using FootballWebApp.Model.Common;
using FootballWebApp.Model.User;
using FootballWebApp.User.Contract;
using Microsoft.AspNetCore.Mvc;

namespace FootballWebApp.User;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserOrchestrator _userOrchestrator;
    private readonly IMapper _mapper;
    
    public UsersController(IUserOrchestrator userOrchestrator, IMapper mapper)
    {
        _userOrchestrator = userOrchestrator;
        _mapper = mapper;
    }
    
    [HttpPost]
    public async Task<IActionResult> PostAsync(PostUser user)
    {
        var userDto = _mapper.Map<UserDto>(user);
        var createdUser = await _userOrchestrator.CreateAsync(userDto);
        
        return Ok(createdUser);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] Pagination pagination)
    {
        var paginationDto = _mapper.Map<PaginationDto>(pagination);
        var users = await _userOrchestrator.GetAllUsersAsync(paginationDto);
        
        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var user = await _userOrchestrator.GetUserByIdAsync(id);
        
        return Ok(user);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] PostUser updatedUser)
    {
        var userDto = _mapper.Map<UserDto>(updatedUser);
        var user = await _userOrchestrator.UpdateUserAsync(id, userDto);
       
        return Ok(user);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deletedUserId = await _userOrchestrator.DeleteUserAsync(id);
        
        return Ok(deletedUserId);
    }
    
}