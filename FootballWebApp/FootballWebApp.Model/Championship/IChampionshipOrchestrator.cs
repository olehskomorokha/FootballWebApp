using FootballWebApp.Model.Common;

namespace FootballWebApp.Model.Championship;

public interface IChampionshipOrchestrator
{
    Task<ChampionshipDto> CreateAsync(ChampionshipDto championshipDto);
    Task<List<ChampionshipDto>> GetAllAsync(PaginationDto pagination);
    Task<ChampionshipDto> GetByIdAsync(Guid id);
    Task<ChampionshipDto> UpdateAsync(Guid id, ChampionshipDto championshipDto);
    Task<Guid> SoftDeleteAsync(Guid id);
}