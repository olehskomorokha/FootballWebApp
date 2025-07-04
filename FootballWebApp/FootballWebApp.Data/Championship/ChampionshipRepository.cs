using AutoMapper;
using FootballWebApp.Model.Championship;
using FootballWebApp.Model.Common;
using Microsoft.EntityFrameworkCore;

namespace FootballWebApp.Data.Championship;

public class ChampionshipRepository : IChampionshipRepository
{
    private readonly IMapper _mapper;
    private readonly CosmosDbContext _cosmosDbcontext;
    public ChampionshipRepository(IMapper mapper, CosmosDbContext context)
    {
        _cosmosDbcontext = context;
        _mapper = mapper;
    }
    
    public async Task<ChampionshipDto> CreateAsync(ChampionshipDto championshipDto)
    {
        var championshipDao = _mapper.Map<ChampionshipDao>(championshipDto);
        var createdEntity = await _cosmosDbcontext.Championships.AddAsync(championshipDao);
        await _cosmosDbcontext.SaveChangesAsync();
        return _mapper.Map<ChampionshipDto>(createdEntity.Entity);
    }
    
    public async Task<List<ChampionshipDto>> GetAllAsync(PaginationDto pagination)
    {
        var championships = await _cosmosDbcontext.Championships
            .Where(x => !x.Deleted)
            .Skip((pagination.page - 1) * pagination.pageSize)
            .Take(pagination.pageSize).ToListAsync();
        return _mapper.Map<List<ChampionshipDto>>(championships);
    }
    
    public async Task<ChampionshipDto> GetByIdAsync(Guid id)
    {
        var championship = await _cosmosDbcontext.Championships.FirstOrDefaultAsync(c => c.Id == id);
        return _mapper.Map<ChampionshipDto>(championship);
    }
    
    public async Task<ChampionshipDto> UpdateAsync(Guid id, ChampionshipDto updatedChampionship)
    {
        var championship = _mapper.Map<ChampionshipDao>(updatedChampionship);
        _cosmosDbcontext.Championships.Update(championship);
        await _cosmosDbcontext.SaveChangesAsync();
        
        return _mapper.Map<ChampionshipDto>(championship);
    }
    
    public async Task<Guid> DeleteAsync(Guid id)
    {
        _cosmosDbcontext.Remove(id);
        await _cosmosDbcontext.SaveChangesAsync();
        return id;
    }
}