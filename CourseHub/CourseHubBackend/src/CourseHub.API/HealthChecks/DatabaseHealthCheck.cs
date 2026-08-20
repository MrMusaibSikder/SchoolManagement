using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CourseHub.API.HealthChecks;

/// <summary>
/// Checks that the configured PostgreSQL database is reachable. Written
/// by hand against EF Core's Database.CanConnectAsync() instead of
/// pulling in AspNetCore.HealthChecks.NpgSql — that package adds nothing
/// this doesn't already need (CourseHubDbContext is already registered),
/// and avoiding an extra dependency keeps the restore graph smaller.
/// Tagged "ready" — see Program.cs for how liveness vs readiness are
/// split.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly CourseHubDbContext _dbContext;

    public DatabaseHealthCheck(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy("Database connection succeeded.")
                : HealthCheckResult.Unhealthy("Database connection failed.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection threw an exception.", ex);
        }
    }
}
