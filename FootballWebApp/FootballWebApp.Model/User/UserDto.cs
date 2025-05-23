using System.Text.Json.Serialization;

namespace FootballWebApp.Model.User;

public class UserDto
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public DateTime DateOfRegistration { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    [JsonIgnore]
    public bool Deleted { get; set; }
}