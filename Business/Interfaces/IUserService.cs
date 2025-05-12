#region

using Business.Models;
using Data.Entities;

#endregion

namespace Business.Interfaces;

public interface IUserService
{
    Task<List<User>> GetAllUsers(int page, int pageSize);
    Task<User> GetUserById(int id);
    Task Register(UserRegisterModel model);
    Task<string> Login(UserLoginModel userLogin);
    Task UpdateUser(int id, UpdateUserModel updatedUser);
    Task DeleteUser(int id);
}