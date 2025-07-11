using FootballWebApp.Data.Championship;
using Microsoft.EntityFrameworkCore;

namespace FootballWebApp.Data;

public class CosmosDbContext : DbContext
{
    public CosmosDbContext(DbContextOptions<CosmosDbContext> options) : base(options)
    {
    }
    
    public DbSet<ChampionshipDao> Championships { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultContainer("championships");
        modelBuilder.Entity<ChampionshipDao>()
            .HasKey(s => s.Id);
        modelBuilder.Entity<ChampionshipDao>()
            .HasPartitionKey(s => s.Id);
        
    }
}