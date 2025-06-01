using FootballWebApp.Model.Championship;
using FootballWebApp.Model.User;
using FootballWebApp.Model.UserChampionships;
using FootballWebApp.Platform.BlobStorage;

namespace FootballWebApp.Orchestrator.UserChampionship;

public class UserChampionshipOrchestrator : IUserChampionshipOrchestrator
{
    private readonly IUserOrchestrator _userOrchestrator;
    private readonly IChampionshipOrchestrator _championshipOrchestrator;
    private readonly IBlobStorage _userChampionshipStorage;
    public UserChampionshipOrchestrator(IUserOrchestrator userOrchestrator, IChampionshipOrchestrator championshipOrchestrator, IBlobStorage userChampionshipStorage)
    {
        _userChampionshipStorage = userChampionshipStorage;
        _userOrchestrator = userOrchestrator;   
        _championshipOrchestrator = championshipOrchestrator;
    }
    public async Task<Model.UserChampionships.UserChampionship> CreateAsync(Guid championshipId, int userId)
    {
        var user = await _userOrchestrator.GetUserByIdAsync(userId);
        var champ = await _championshipOrchestrator.GetByIdAsync(championshipId);
        
        var fileName = $"{championshipId:N}_{userId}";

        var exists = await _userChampionshipStorage.ContainsFileByNameAsync(fileName);
        
        if (!exists)
        {
            await _userChampionshipStorage.CreateFileAsync(fileName);
        }

        return new Model.UserChampionships.UserChampionship()
        {
            ChampionshipId = championshipId,
            UserId = userId
        };
    }
    public async Task<List<int>> GetAllAsync(Guid championshipId)
    {
        var championship = await _championshipOrchestrator.GetByIdAsync(championshipId);
        return await _userChampionshipStorage.GetAllFilesByNameAsync(championshipId);
    }
    
    public async Task<Stream> GetUserChampionshipAsync(Guid championshipId, int userId)
    {
        var user = await _userOrchestrator.GetUserByIdAsync(userId);
        var championship = await _championshipOrchestrator.GetByIdAsync(championshipId);
        
        var fileName = $"{championshipId:N}_{userId}";
        return await _userChampionshipStorage.GetFileByNameAsync(fileName);
    }

    public async Task DeleteUserChampionshipAsync(Guid championshipId, int userId)
    {
        var user = await _userOrchestrator.GetUserByIdAsync(userId);
        var championship = await _championshipOrchestrator.GetByIdAsync(championshipId);
        
        var fileName = $"{championshipId:N}_{userId}";
        await _userChampionshipStorage.DeleteFileAsync(fileName);
    }

    public async Task UpdateUserChampionshipAsync(Guid championshipId, int userId, Stream content)
    {
        var user = await _userOrchestrator.GetUserByIdAsync(userId);
        var championship = await _championshipOrchestrator.GetByIdAsync(championshipId);
        
        var fileName = $"{championshipId:N}_{userId}";
        await _userChampionshipStorage.UpdateFileAsync(fileName, content);
    }
}