using FootballWebApp.Model.Common;

namespace FootballWebApp.Model.User;

public interface IUserOrchestrator
{
    Task<UserDto> CreateAsync(UserDto user);
    Task<List<UserDto>> GetAllUsersAsync(PaginationDto pagination);
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> UpdateUserAsync(int id, UserDto updatedUser);
    Task<int> DeleteUserAsync(int id);
}