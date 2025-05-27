using AutoMapper;
using FootballWebApp.Data.Championship;
using FootballWebApp.Model.Championship;

namespace FootballWebApp.Championship.Contract;

public class ChampionshipMap : Profile
{
    public ChampionshipMap()
    {
        CreateMap<ChampionshipDto, ChampionshipDao>().ReverseMap();
        CreateMap<CreateChampionship, ChampionshipDto>()
            .ForMember(dest => dest.Id, src =>src.MapFrom(_ => Guid.NewGuid()));
    }
    
}