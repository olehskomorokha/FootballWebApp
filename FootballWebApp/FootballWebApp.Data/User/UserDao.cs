using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballWebApp.Data.User;

[Table("users")]
public class UserDao
{
    [Column("id"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Column("nickName")]
    public string Nickname { get; set; }
    [Column("dateOfRegistration")]
    public DateTime DateOfRegistration { get; set; }
    [Column("email")]
    public string Email { get; set; }
    [Column("password")]
    public string Password { get; set; }
    [Column("deleted")]
    public bool Deleted { get; set; }
}