using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace CourseHub.IntegrationTests;

/// <summary>
/// Overrides just enough configuration to make the integration test
/// suite self-contained and deterministic, regardless of what (if
/// anything) a developer has set in their local User Secrets:
/// - A fixed, test-only JWT signing key, so tests never depend on
///   Authentication:Jwt:SecretKey being configured locally.
/// - A fixed, known SuperAdminInviteCode, so tests can always bootstrap a
///   SuperAdmin account deterministically to exercise permission-gated
///   admin endpoints end-to-end (see TestHelpers.CreateSuperAdminClientAsync).
///
/// Still requires a real, reachable PostgreSQL database — the connection
/// string itself is NOT overridden here, so it still comes from
/// appsettings.Development.json / User Secrets as documented in the
/// README's "Local setup" section. Startup applies pending migrations
/// and reseeds automatically (see Program.cs), so no manual `dotnet ef
/// database update` step is needed before running these tests either.
/// </summary>
public class CourseHubApiFactory : WebApplicationFactory<Program>
{
    public const string SuperAdminInviteCode = "integration-tests-super-admin-code";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Jwt:SecretKey"] = "integration-tests-only-jwt-secret-key-not-for-production-use-32bytes-min",
                ["Seed:SuperAdminInviteCode"] = SuperAdminInviteCode,
            });
        });

        base.ConfigureWebHost(builder);
    }
}
