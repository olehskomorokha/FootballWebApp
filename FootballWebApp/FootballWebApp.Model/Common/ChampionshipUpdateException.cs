namespace FootballWebApp.Model.Common;

public class ChampionshipUpdateException : ApiException
{
    public ChampionshipUpdateException()
        : base("Update exception", "Championship update not found")
    {
    }
    
}