namespace FootballWebApp.Model.User;

public interface IUserRepository
{
    Task<UserDto> CreateAsync(UserDto user);
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> UpdateUserAsync(int id, UserDto updatedUser);
    Task<int> DeleteUserAsync(int id);
}