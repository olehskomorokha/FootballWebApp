using AutoMapper;
using FootballWebApp.Model.Common;

namespace FootballWebApp.User.Contract;

public class PaginationMap : Profile
{
    public PaginationMap()
    {
        CreateMap<Pagination, PaginationDto>();
        CreateMap<PaginationDto, Pagination>();
    }
}