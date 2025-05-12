#region

using Microsoft.Extensions.Configuration;

#endregion

namespace Data.Data;

public class AppConfiguration
{
    public string sqlConnectionString { get; set; }

    public AppConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        var basePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
        var path = Path.Combine(basePath, "FootballTournamentApp", "appsettings.json");
        configBuilder.AddJsonFile(path, false);
        var root = configBuilder.Build();
        var appSettings = root.GetSection("ConnectionStrings:DefaultConnection");
        sqlConnectionString = appSettings.Value;
    }
}