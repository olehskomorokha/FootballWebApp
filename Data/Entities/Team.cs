#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace Data.Entities;

public class Team
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public ICollection<UserTeam> userTeams { get; set; }
    public ICollection<GameResult> gameResults { get; set; }
}