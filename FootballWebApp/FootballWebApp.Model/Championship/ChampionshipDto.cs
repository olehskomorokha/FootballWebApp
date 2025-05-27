using System.Text.Json.Serialization;

namespace FootballWebApp.Model.Championship;

public class ChampionshipDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime DateOfCreation { get; set; }
    public int TeamAPoints { get; set; }
    public int TeamBPoints { get; set; }
    public int TeamCPoints { get; set; }
    [JsonIgnore]
    public bool Deleted { get; set; }
}