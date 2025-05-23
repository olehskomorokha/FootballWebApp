using AutoMapper;
using FootballWebApp.Data.User;
using FootballWebApp.Model.User;

namespace FootballWebApp.User.Contract;

public class UserMap : Profile
{
    public UserMap()
    {
        CreateMap<PostUser, UserDto>();
        CreateMap<UserDto, UserDao>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UserDao, UserDto>();
        
    }
}