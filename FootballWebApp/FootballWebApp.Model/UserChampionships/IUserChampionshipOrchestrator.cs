namespace FootballWebApp.Model.UserChampionships;

public interface IUserChampionshipOrchestrator
{
    Task<UserChampionship> CreateAsync(Guid championshipId, int userId);
    Task<List<int>> GetAllAsync(Guid championshipId);
    Task<Stream> GetUserChampionshipAsync(Guid championshipId, int userId);
    Task DeleteUserChampionshipAsync(Guid championshipId, int userId);
    Task UpdateUserChampionshipAsync(Guid championshipId, int userId, Stream content);
}