namespace Data.Entities;

public class GameResult
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int TeamId { get; set; }
    public int GameScore { get; set; }
    
    public Team Team { get; set; }
    public Game Game { get; set; }
    
}