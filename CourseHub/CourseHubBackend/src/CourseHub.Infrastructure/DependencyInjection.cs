using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Options;
using CourseHub.Infrastructure.Authentication;
using CourseHub.Infrastructure.Persistence.Context;
using CourseHub.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CourseHub.Infrastructure;

public static class DependencyInjection
{
    private const string DefaultConnectionName = "DefaultConnection";

    /// <summary>
    /// Registers Infrastructure-layer services — persistence (DbContext,
    /// repositories, unit of work) and the concrete authentication
    /// primitives (password hashing, JWT issuance, secure token
    /// generation/hashing) — with the DI container. Call from
    /// CourseHub.API: builder.Services.AddInfrastructure(builder.Configuration);
    ///
    /// Does NOT register IEmailSender or configure ASP.NET Core's JWT
    /// bearer authentication middleware — those are environment-specific /
    /// web-hosting concerns and are registered in CourseHub.API instead
    /// (see Program.cs and Extensions/AuthenticationExtensions.cs).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DefaultConnectionName);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{DefaultConnectionName}' was not found in configuration.");
        }

        services.AddDbContext<CourseHubDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<PasswordResetOptions>(configuration.GetSection(PasswordResetOptions.SectionName));
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));

        services.AddScoped<IUnitOfWork, Persistence.UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IInstitutionRepository, InstitutionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

        services.AddSingleton<IPasswordHasher, PasswordHasherAdapter>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<ISecureTokenGenerator, SecureTokenGenerator>();
        services.AddSingleton<ITokenHasher, Sha256TokenHasher>();

        return services;
    }
}
