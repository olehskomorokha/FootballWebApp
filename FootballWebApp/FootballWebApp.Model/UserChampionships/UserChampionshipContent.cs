using System.ComponentModel.DataAnnotations;

namespace FootballWebApp.Model.UserChampionships;

public class UserChampionshipContent
{
    [Required]
    public DateTime JoinDate { get; set; }
    
    [Required]
    public int Score { get; set; }
    
    [Required]
    public string Role { get; set; }
    
    public string Notes { get; set; }
    
    [Required]
    public bool IsActive { get; set; }
    
    public List<MatchParticipation> Matches { get; set; } = new();
}

public class MatchParticipation
{
    [Required]
    public DateTime MatchDate { get; set; }
    
    [Required]
    public int GoalsScored { get; set; }
    
    [Required]
    public int Assists { get; set; }
    
    public string Position { get; set; }
} 