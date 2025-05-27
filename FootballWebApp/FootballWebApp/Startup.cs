using FootballWebApp.Championship.Contract;
using FootballWebApp.Data;
using FootballWebApp.Data.Championship;
using FootballWebApp.Data.User;
using FootballWebApp.Model.Championship;
using FootballWebApp.Model.User;
using FootballWebApp.Orchestrator.Championship;
using FootballWebApp.Orchestrators.User;
using FootballWebApp.User.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FootballWebApp;

public static class ApplicationBuilderExtensions
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CosmosDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
}

public class Startup
{
    private IConfiguration _configuration { get; }

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Реєстрація сервісів
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddScoped<IChampionshipRepository, ChampionshipRepository>();
        services.AddScoped<IChampionshipOrchestrator, ChampionshipOrchestrator>();
        services.AddScoped<IUserOrchestrator, UserOrchestrator>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddAutoMapper(config => config.AddProfile(new UserMap()));
        services.AddAutoMapper(config => config.AddProfile(new PaginationMap()));
        services.AddAutoMapper(config => config.AddProfile(new ChampionshipMap()));

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });
        
        ConfigureDb(services);
    }

    // Налаштування middleware
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // Initialize database asynchronously
        app.InitializeDatabaseAsync().GetAwaiter().GetResult();
    }

    public virtual void ConfigureDb(IServiceCollection services)
    {
        services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<CosmosDbContext>(options =>
            options.UseCosmos(_configuration.GetConnectionString("CosmosConnection"),
                databaseName: "FootballWebApp"));
    }
}