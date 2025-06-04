using System.ComponentModel.DataAnnotations;

namespace FootballWebApp.Championship.Contract;

public class CreateChampionship
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Date of creation is required.")]
    [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
    public DateTime DateOfCreation { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Team A points must be 0 or greater.")]
    public int TeamAPoints { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Team A points must be 0 or greater.")]
    public int TeamBPoints { get; set; }
    [Range(0, int.MaxValue, ErrorMessage = "Team A points must be 0 or greater.")]
    public int TeamCPoints { get; set; }
}