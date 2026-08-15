using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CourseHub.Infrastructure.Persistence.Context;

/// <summary>
/// Lets `dotnet ef migrations` create/apply migrations at design time,
/// without needing the full API host to start up. Reads the connection
/// string the same way the running API does: environment variable first,
/// then CourseHub.API/appsettings.json (and appsettings.Development.json
/// if present) as a fallback. No production secrets live here.
/// </summary>
public class CourseHubDbContextFactory : IDesignTimeDbContextFactory<CourseHubDbContext>
{
    private const string ConnectionStringEnvVar = "ConnectionStrings__DefaultConnection";

    public CourseHubDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvVar)
            ?? ReadConnectionStringFromApiAppSettings()
            ?? throw new InvalidOperationException(
                "No database connection string found. Set the " +
                $"{ConnectionStringEnvVar} environment variable, or add " +
                "ConnectionStrings:DefaultConnection to src/CourseHub.API/appsettings.json.");

        var optionsBuilder = new DbContextOptionsBuilder<CourseHubDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new CourseHubDbContext(optionsBuilder.Options);
    }

    private static string? ReadConnectionStringFromApiAppSettings()
    {
        var apiProjectDirectory = FindApiProjectDirectory();
        if (apiProjectDirectory is null)
        {
            return null;
        }

        // appsettings.Development.json (if present) takes precedence over
        // appsettings.json, mirroring ASP.NET Core's own configuration order.
        return ReadConnectionString(Path.Combine(apiProjectDirectory, "appsettings.Development.json"))
            ?? ReadConnectionString(Path.Combine(apiProjectDirectory, "appsettings.json"));
    }

    private static string? ReadConnectionString(string appSettingsPath)
    {
        if (!File.Exists(appSettingsPath))
        {
            return null;
        }

        using var stream = File.OpenRead(appSettingsPath);
        using var document = JsonDocument.Parse(stream);

        if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings) &&
            connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection) &&
            defaultConnection.ValueKind == JsonValueKind.String)
        {
            return defaultConnection.GetString();
        }

        return null;
    }

    /// <summary>
    /// Walks up from the current directory looking for
    /// src/CourseHub.API/appsettings.json, so this works whether `dotnet ef`
    /// is invoked with a working directory of the solution root, the
    /// Infrastructure project, or the API project itself.
    /// </summary>
    private static string? FindApiProjectDirectory()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "src", "CourseHub.API");
            if (File.Exists(Path.Combine(candidate, "appsettings.json")))
            {
                return candidate;
            }

            // Handle being invoked from inside src/CourseHub.API already.
            if (directory.Name == "CourseHub.API" && File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
