#region

using Business.Helpers;
using Business.Interfaces;
using Business.Mapper;
using Business.Models;
using Data.Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Business.Services;

public class UserService : IUserService
{
    private readonly StoreDbContext _context;
    private readonly PasswordHasher _passwordHasher;

    public UserService(StoreDbContext context, PasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
        _context = context;
    }

    public async Task<List<User>> GetAllUsers(int page = 1, int pageSize = 10)
    {
        return await _context.Users.Where(x => !x.Deleted).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null || user.Deleted)
        {
            throw new Exception($"User with id {id} not found");
        }

        return user;
    }

    public async Task Register(UserRegisterModel model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password) ||
            string.IsNullOrWhiteSpace(model.NickName))
        {
            throw new ArgumentException("Not all data fields are filled");
        }

        var exists = await _context.Users
            .AnyAsync(u => u.Email == model.Email || u.Nickname == model.NickName);

        if (exists)
        {
            throw new InvalidOperationException("User with the same email or nickname already exists.");
        }

        model.Password = _passwordHasher.HashPassword(model.Password);
        await _context.Users.AddAsync(UserMapper.MapToUserRegisterModel(model));
        await _context.SaveChangesAsync();
    }

    public async Task<string> Login(UserLoginModel userLogin)
    {
        if (userLogin == null)
        {
            throw new ArgumentNullException(nameof(userLogin), message: "userLogin model is null");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userLogin.Email);

        if (user == null)
        {
            throw new ApplicationException("Wrong email");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(user.Password, userLogin.Password);

        if (!isPasswordValid)
        {
            throw new ApplicationException("Wrong password");
        }

        var token = _passwordHasher.GetAccessToken(userLogin.Email, userLogin.Password);
        return token;
    }

    public async Task UpdateUser(int id, UpdateUserModel updatedData)
    {
        var user = await GetUserById(id);
        user.Nickname = updatedData.Nickname;
        user.Email = updatedData.Email;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(int id)
    {
        var user = await GetUserById(id);
        user.Deleted = true;
        await _context.SaveChangesAsync();
    }
}