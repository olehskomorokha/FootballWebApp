using FootballWebApp.Data.User;
using Microsoft.EntityFrameworkCore;

namespace FootballWebApp.Data;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
    {
    }

    public DbSet<UserDao> Users { get; set; }
}