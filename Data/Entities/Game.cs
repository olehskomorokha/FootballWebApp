namespace Data.Entities;

public class Game
{
    public int Id { get; set; }
    public int TeamA { get; set; }
    public int TeamB { get; set; }
    public int TeamC { get; set; }
    public int ChampionshipId { get; set; }
    
    public Championship Championship { get; set; }
    
    
}