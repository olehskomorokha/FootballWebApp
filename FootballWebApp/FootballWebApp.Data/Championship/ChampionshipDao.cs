using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Cosmos;

namespace FootballWebApp.Data.Championship;

public class ChampionshipDao
{
    [Column("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public Guid Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("dateOfCreation")]
    public DateTime DateOfCreation { get; set; }
    [Column("teamAPoints")]
    public int TeamAPoints { get; set; }
    [Column("teamBPoints")]
    public int TeamBPoints { get; set; }
    [Column("teamCPoints")]
    public int TeamCPoints { get; set; }
    [Column("deleted")]
    public bool Deleted { get; set; }
}