using FootballWebApp.Model.Championship;
using FootballWebApp.Model.Common;

namespace FootballWebApp.Orchestrator.Championship;

public class ChampionshipOrchestrator : IChampionshipOrchestrator
{
    private readonly IChampionshipRepository _repository;
    public ChampionshipOrchestrator(IChampionshipRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ChampionshipDto> CreateAsync(ChampionshipDto championshipDto)
    {
        var championships = await _repository.GetAllAsync(new PaginationDto { page = 1, pageSize = int.MaxValue });
        if (championships.Any(x => x.Name == championshipDto.Name && !x.Deleted))
        {
            throw new ChampionshipAlreadyExistsException();
        }
        championshipDto.DateOfCreation = DateTime.UtcNow;
        return await _repository.CreateAsync(championshipDto);
    }
    
    public async Task<List<ChampionshipDto>> GetAllAsync(PaginationDto pagination)
    {
        return await _repository.GetAllAsync(pagination);
    }
    
    public async Task<ChampionshipDto> GetByIdAsync(Guid id)
    {
        var championship = await _repository.GetByIdAsync(id);
        if (championship == null || championship.Deleted)
        {
            throw new ChampionshipNotFoundException();
        }
        return championship;
    }
    
    public async Task<ChampionshipDto> UpdateAsync(Guid id, ChampionshipDto updatedChampionship)
    {
        await GetByIdAsync(id);
        var updated = await _repository.UpdateAsync(id, updatedChampionship);
        
        return updated;
    }
    
    public async Task<Guid> SoftDeleteAsync(Guid id)
    {
        var championship = await GetByIdAsync(id);
        championship.Deleted = true;
        await _repository.UpdateAsync(id, championship);
        return id;
    }
}