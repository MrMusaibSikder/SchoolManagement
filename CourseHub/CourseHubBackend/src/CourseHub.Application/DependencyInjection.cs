using CourseHub.Application.Features.Auth;
using CourseHub.Application.Features.Auth.Dtos;
using CourseHub.Application.Features.Auth.Validators;
using CourseHub.Application.Features.Permissions;
using CourseHub.Application.Features.Permissions.Dtos;
using CourseHub.Application.Features.Permissions.Validators;
using CourseHub.Application.Features.Public;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CourseHub.Application;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Application-layer services: use-case orchestrators and
    /// request validators. No configuration/infrastructure concerns here —
    /// call alongside AddInfrastructure(configuration) from CourseHub.API:
    /// builder.Services.AddApplication();
    /// builder.Services.AddInfrastructure(builder.Configuration);
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPublicInstitutionService, PublicInstitutionService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();

        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();
        services.AddScoped<IValidator<LogoutRequest>, LogoutRequestValidator>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
        services.AddScoped<IValidator<ForgotPasswordRequest>, ForgotPasswordRequestValidator>();
        services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordRequestValidator>();
        services.AddScoped<IValidator<AssignPermissionRequest>, AssignPermissionRequestValidator>();

        return services;
    }
}
