using System.Text;
using CourseHub.Application.Common.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CourseHub.API.Extensions;

/// <summary>
/// Configures ASP.NET Core JWT Bearer authentication. Kept in the API
/// project because AddAuthentication/AddJwtBearer are web-hosting concerns,
/// not persistence/business logic — Infrastructure only provides the
/// JwtOptions binding and the token *issuing* service (IJwtTokenService);
/// this extension wires up token *validation* for incoming requests.
/// </summary>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey) || jwtOptions.SecretKey.Length < 32)
        {
            throw new InvalidOperationException(
                $"'{JwtOptions.SectionName}:SecretKey' is missing or too short (minimum 32 characters). " +
                "Set it via User Secrets in development (dotnet user-secrets set " +
                $"\"{JwtOptions.SectionName}:SecretKey\" \"<value>\") or an environment variable/secret " +
                "manager in production. It must never be committed to appsettings.json.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization();

        return services;
    }
}
