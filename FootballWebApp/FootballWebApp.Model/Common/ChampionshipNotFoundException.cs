namespace FootballWebApp.Model.Common;

public class ChampionshipNotFoundException : ApiException
{
    public ChampionshipNotFoundException()
        : base("notFound", "Championship not found")
    {
    }
} 