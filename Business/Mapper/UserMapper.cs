#region

using Business.Models;
using Data.Entities;

#endregion

namespace Business.Mapper;

public static class UserMapper
{
    public static User MapToUserRegisterModel(UserRegisterModel userRegisterModel)
    {
        return new User
        {
            Email = userRegisterModel.Email,
            Nickname = userRegisterModel.NickName,
            Password = userRegisterModel.Password,
            DateOfRegistration = DateTime.Now
        };
    }
}