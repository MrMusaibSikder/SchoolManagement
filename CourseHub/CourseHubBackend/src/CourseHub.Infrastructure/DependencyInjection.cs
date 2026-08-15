using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseHub.Infrastructure;

public static class DependencyInjection
{
    private const string DefaultConnectionName = "DefaultConnection";

    /// <summary>
    /// Registers Infrastructure-layer services (currently: the PostgreSQL
    /// DbContext) with the DI container. Call from CourseHub.API:
    /// builder.Services.AddInfrastructure(builder.Configuration);
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnectionName)
            ?? throw new InvalidOperationException(
                $"Connection string '{DefaultConnectionName}' was not found in configuration.");

        services.AddDbContext<CourseHubDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
