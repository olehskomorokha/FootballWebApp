namespace FootballWebApp.Model.Common;

public class ChampionshipAlreadyExistsException : ApiException
{
    public ChampionshipAlreadyExistsException()
        : base("alreadyExist", "Championship already exists")
    {
    }
} 