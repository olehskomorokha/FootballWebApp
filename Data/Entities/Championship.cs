namespace Data.Entities;

public class Championship
{
    public int Id { get; set; }
    public DateTime DateOfCreation { get; set; }
    public ICollection<ChampionshipTeam> Teams { get; set; }
    public ICollection<Game> Games { get; set; }
}