using FootballWebApp.Championship.Contract;
using FootballWebApp.Data;
using FootballWebApp.Data.Championship;
using FootballWebApp.Model.Championship;
using FootballWebApp.Orchestrator.Championship;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FootballWebApp.IntegrationTests;

public class TestStartup
{
    public IConfiguration Configuration { get; }

    public TestStartup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddAutoMapper(typeof(ChampionshipMap));

        
        services.AddDbContext<CosmosDbContext>(options =>
        {
            options.UseInMemoryDatabase("TestFootballWebAppCosmosDb");
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        services.AddScoped<IChampionshipRepository, ChampionshipRepository>();
        services.AddScoped<IChampionshipOrchestrator, ChampionshipOrchestrator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
} 