namespace FootballWebApp.Model.Championship;

public interface IChampionshipRepository
{
    Task<ChampionshipDto> CreateAsync(ChampionshipDto championshipDto);
    Task<List<ChampionshipDto>> GetAllAsync();
    Task<ChampionshipDto> GetByIdAsync(Guid id);
    Task<ChampionshipDto> UpdateAsync(Guid id, ChampionshipDto championshipDto);
    Task<Guid> DeleteAsync(Guid id);
    
}