#region

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Data.Entities;

[Table("Users")]
public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Nickname { get; set; }
    public DateTime DateOfRegistration { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; }

    [DefaultValue(false)]
    public bool Deleted { get; set; }

    [AllowNull]
    public ICollection<UserTeam> UserTeams { get; set; }
}