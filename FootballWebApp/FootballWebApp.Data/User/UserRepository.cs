using AutoMapper;
using FootballWebApp.Model.User;
using Microsoft.EntityFrameworkCore;

namespace FootballWebApp.Data.User;

public class UserRepository : IUserRepository
{
    private readonly SqlDbContext _context;
    private readonly IMapper _mapper;
    public UserRepository(SqlDbContext sqlDbContext, IMapper mapper)
    {
        _context = sqlDbContext;
        _mapper = mapper;
    }

    public async Task<UserDto> CreateAsync(UserDto user)
    {
        var userDao = _mapper.Map<UserDao>(user);
        
        var createdUser = await _context.Users.AddAsync(userDao);
        await _context.SaveChangesAsync();
        var userDto = _mapper.Map<UserDto>(createdUser.Entity);
        
        return userDto;
    }
    
    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<List<UserDto>>(users);
    }
    
    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return _mapper.Map<UserDto>(user);
    }
    
    public async Task<UserDto> UpdateUserAsync(int id, UserDto updatedUser)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id);
        _mapper.Map(updatedUser,user);
        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }
    
    public async Task<int> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FirstAsync(u => u.Id == id);
        user.Deleted = true;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user.Id;
    }
}