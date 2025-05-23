using FootballWebApp.Model.Common;
using FootballWebApp.Model.User;

namespace FootballWebApp.Orchestrators.User;

public class UserOrchestrator : IUserOrchestrator
{ 
    private readonly IUserRepository _userRepository;
    public UserOrchestrator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<UserDto> CreateAsync(UserDto user)
    {
        var users= await _userRepository.GetAllUsersAsync();
        if (users.Any(x => x.Email == user.Email))
        {
            throw new UserAlreadyExistsException();
        }
        user.DateOfRegistration = DateTime.UtcNow;
        return await _userRepository.CreateAsync(user); 
    }
    
    public async Task<List<UserDto>> GetAllUsersAsync(PaginationDto pagination)
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Where(x => !x.Deleted).Skip((pagination.page - 1) * pagination.pageSize).Take(pagination.pageSize).ToList();
    }
    
    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null || user.Deleted)
        {
            throw new UserNotFoundException();
        }
        return user;
    }
    
    public async Task<UserDto> UpdateUserAsync(int id, UserDto updatedUser)
    {
        await GetUserByIdAsync(id);
        var users= await _userRepository.GetAllUsersAsync();
        var updated = await _userRepository.UpdateUserAsync(id, updatedUser);
        if (users.Any(x => x.Email == updatedUser.Email))
        {
            throw new UserAlreadyExistsException();
        }
        return updated;
    }
    
    public async Task<int> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        if (user == null)
        {
            throw new UserNotFoundException();
        }
        return await _userRepository.DeleteUserAsync(id);
    }
}