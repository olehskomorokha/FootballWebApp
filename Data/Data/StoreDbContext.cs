#region

using Data.Entities;
using Microsoft.EntityFrameworkCore;

#endregion

namespace Data.Data;

public class StoreDbContext : DbContext
{
    public static OptionsBuild optionsBuild = new OptionsBuild();

    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserTeam> UserTeams { get; set; }
    public DbSet<Championship> Championships { get; set; }
    public DbSet<ChampionshipTeam> ChampionshipTeams { get; set; }
    public DbSet<GameResult> GameResults { get; set; }
    public DbSet<Game> Games { get; set; }
}

public class OptionsBuild
{
    private readonly AppConfiguration settings;
    public DbContextOptions<StoreDbContext> dbOptions { get; set; }
    public DbContextOptionsBuilder<StoreDbContext> opsBuilder { get; set; }

    public OptionsBuild()
    {
        settings = new AppConfiguration();
        opsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
        opsBuilder.UseSqlServer(settings.sqlConnectionString);
        dbOptions = opsBuilder.Options;
    }
}