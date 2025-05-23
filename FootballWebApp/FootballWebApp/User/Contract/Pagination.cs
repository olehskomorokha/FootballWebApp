using System.ComponentModel.DataAnnotations;

namespace FootballWebApp.User.Contract;

public class Pagination
{
    [Range(1, int.MaxValue)]
    public int page { get; set; }
    [Range(1, int.MaxValue)]
    public int pageSize { get; set; }
}