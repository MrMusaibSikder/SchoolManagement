using System.Reflection;
using CourseHub.API.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CourseHub.API.Extensions;

/// <summary>
/// Phase 13: full Swagger/OpenAPI documentation.
/// - JWT Bearer security definition, so the "Authorize" button in Swagger
///   UI can be used to test authenticated endpoints.
/// - A top-level description covering auth flow, the permission model,
///   and the error response shape — the things someone needs to know
///   once, rather than repeated on every endpoint.
/// - XML doc comments pulled from BOTH this project (controller/action
///   &lt;summary&gt; text) and CourseHub.Application (DTO/record &lt;summary&gt;
///   text) — see the corresponding &lt;GenerateDocumentationFile&gt; in each
///   .csproj.
/// - Realistic example request bodies for the most commonly-used
///   endpoints (see RequestExampleOperationFilter).
/// </summary>
public static class SwaggerServiceExtensions
{
    private const string Description = """
        Single-institute Course & Training Management API. See the project
        README for the full picture — this is the short version.

        ## Authentication
        1. `POST /api/auth/register` (or `/api/auth/login`) returns an
           `accessToken` and a `refreshToken`.
        2. Click **Authorize** above and enter `Bearer {accessToken}` (the
           word "Bearer", a space, then the token) — Swagger UI then sends
           it as the `Authorization` header on every request you try here.
        3. Access tokens are short-lived. When one expires, call
           `POST /api/auth/refresh` with the `refreshToken` to get a new
           pair, without logging in again.

        ## Roles vs. permissions
        Every endpoint under `/api/admin/*` requires a specific permission
        (shown per-endpoint below as e.g. `courses.create`), not just a
        role name. `SuperAdmin` always has every permission. Other roles
        (`Admin`, `Teacher`, `Student`) only have whatever's been assigned
        via `/api/admin/roles/{roleId}/permissions`. A role's permission
        changes only take effect on that user's *next* login/refresh — the
        JWT carries the permission set at issue time, not looked up live.

        ## Errors
        Every error response (400/401/403/404/500) is an RFC 7807
        `ProblemDetails` JSON object: `{ status, title, detail, instance,
        traceId }`. `detail` is safe to show a user for 4xx responses; for
        500s it's always a generic message — the real exception is only
        ever logged server-side, never returned.

        ## "Promotion" pattern (Teachers/Students)
        A Teacher or Student profile is never created standalone — it
        promotes an existing `User` (who must already hold the matching
        role). `GET /api/admin/teachers/eligible-users` and
        `GET /api/admin/students/eligible-users` list valid candidates.
        """;

    public static IServiceCollection AddSwaggerWithJwtSupport(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CourseHub API",
                Version = "v1",
                Description = Description,
            });

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

            // Pull in <summary> XML doc comments from this project
            // (controllers/actions) and from CourseHub.Application
            // (DTOs/records) so both endpoint descriptions and request/
            // response schema property descriptions show up in the UI.
            // Guarded with File.Exists rather than assuming — a missing
            // XML file should degrade to "no descriptions", not crash
            // Swagger generation at startup.
            var apiXmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(apiXmlPath))
            {
                options.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);
            }

            var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, "CourseHub.Application.xml");
            if (File.Exists(applicationXmlPath))
            {
                options.IncludeXmlComments(applicationXmlPath);
            }

            options.OperationFilter<RequestExampleOperationFilter>();
        });

        return services;
    }
}
