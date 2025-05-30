namespace FootballWebApp.Model.UserChampionships;

public interface IUserChampionshipOrchestrator
{
    Task<UserChampionship> CreateAsync(Guid championshipId, int userId);
    Task<List<int>> GetAllAsync(Guid championshipId);
}