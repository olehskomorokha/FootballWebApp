using System.ComponentModel.DataAnnotations;

namespace FootballWebApp.User.Contract;

public class PostUser
{
    [Required]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Nickname must be between 3 and 50 characters.")]
    public string Nickname { get; set; }
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}