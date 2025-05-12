namespace Data.Entities;

public class ChampionshipTeam
{
    public int Id { get; set; }
    public int ChampionshipId { get; set; }
    public Championship Championship { get; set; }
    public ICollection<Team> Teams { get; set; }
}