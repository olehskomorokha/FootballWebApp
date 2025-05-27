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
        var championships = await _repository.GetAllAsync();
        if (championships.Any(x => x.Name == championshipDto.Name && !x.Deleted))
        {
            throw new ChampionshipAlreadyExistsException();
        }
        championshipDto.DateOfCreation = DateTime.UtcNow;
        return await _repository.CreateAsync(championshipDto);
    }
    
    public async Task<List<ChampionshipDto>> GetAllAsync(PaginationDto pagination)
    {
        var championships = await _repository.GetAllAsync();
        return championships.Where(x => !x.Deleted)
            .Skip((pagination.page - 1) * pagination.pageSize)
            .Take(pagination.pageSize)
            .ToList();
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
        var championships = await _repository.GetAllAsync();
        var updated = await _repository.UpdateAsync(id, updatedChampionship);
        if (championships.Any(x => x.Name == updatedChampionship.Name && x.Id != id && !x.Deleted))
        {
            throw new ChampionshipAlreadyExistsException();
        }

        return updated;
    }
    
    public async Task<Guid> DeleteAsync(Guid id)
    {
        var championship = await _repository.GetByIdAsync(id);
        if (championship == null)
        {
            throw new ChampionshipNotFoundException();
        }
        return await _repository.DeleteAsync(id);
    }
}