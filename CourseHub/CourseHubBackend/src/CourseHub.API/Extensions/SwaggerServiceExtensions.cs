using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CourseHub.API.Extensions;

/// <summary>
/// Adds a JWT Bearer security definition/requirement to Swagger so the
/// "Authorize" button in Swagger UI can be used to test authenticated
/// endpoints (Authorization: Bearer {token}). Full Swagger documentation
/// (grouping, examples, XML comments, etc.) is Phase 13 — this is the
/// minimal addition needed to make Phase 8 testable via Swagger UI.
/// </summary>
public static class SwaggerServiceExtensions
{
    public static IServiceCollection AddSwaggerWithJwtSupport(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter a JWT access token. Example: eyJhbGciOi...",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });

        return services;
    }
}
