using AutoMapper;
using FootballWebApp.Championship.Contract;
using FootballWebApp.Model.Championship;
using FootballWebApp.Model.Common;
using FootballWebApp.User.Contract;
using Microsoft.AspNetCore.Mvc;

namespace FootballWebApp.Championship;

[ApiController]
[Route("api/v1/[controller]")]
public class ChampionshipsController : ControllerBase
{
    private readonly IChampionshipOrchestrator _championshipOrchestrator;
    private readonly IMapper _mapper;

    public ChampionshipsController(IMapper mapper, IChampionshipOrchestrator championshipOrchestrator)
    {
        _championshipOrchestrator = championshipOrchestrator;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(CreateChampionship championship)
    {
        var championshipDto = _mapper.Map<ChampionshipDto>(championship);
        var createdEntity = await _championshipOrchestrator.CreateAsync(championshipDto);
        return Ok(createdEntity);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] Pagination pagination)
    {
        var paginationDto = _mapper.Map<PaginationDto>(pagination);
        var championships = await _championshipOrchestrator.GetAllAsync(paginationDto);
        return Ok(championships);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var championship = await _championshipOrchestrator.GetByIdAsync(id);
        return Ok(championship);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] CreateChampionship championship)
    {
        var championshipDto = _mapper.Map<ChampionshipDto>(championship);
        championshipDto.Id = id;
        var updatedChampionship = await _championshipOrchestrator.UpdateAsync(id, championshipDto);
        return Ok(updatedChampionship);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var deletedId = await _championshipOrchestrator.SoftDeleteAsync(id);
        return Ok(deletedId);
    }
}