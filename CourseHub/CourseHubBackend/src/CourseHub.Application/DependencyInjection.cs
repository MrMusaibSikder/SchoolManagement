using CourseHub.Application.Features.Auth;
using CourseHub.Application.Features.Auth.Dtos;
using CourseHub.Application.Features.Auth.Validators;
using CourseHub.Application.Features.Batches;
using CourseHub.Application.Features.Batches.Dtos;
using CourseHub.Application.Features.Batches.Validators;
using CourseHub.Application.Features.Courses;
using CourseHub.Application.Features.Courses.Dtos;
using CourseHub.Application.Features.Courses.Validators;
using CourseHub.Application.Features.Permissions;
using CourseHub.Application.Features.Permissions.Dtos;
using CourseHub.Application.Features.Permissions.Validators;
using CourseHub.Application.Features.Public;
using CourseHub.Application.Features.Students;
using CourseHub.Application.Features.Students.Dtos;
using CourseHub.Application.Features.Students.Validators;
using CourseHub.Application.Features.Teachers;
using CourseHub.Application.Features.Teachers.Dtos;
using CourseHub.Application.Features.Teachers.Validators;
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
        services.AddScoped<IPublicCatalogService, PublicCatalogService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IBatchService, BatchService>();

        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();
        services.AddScoped<IValidator<LogoutRequest>, LogoutRequestValidator>();
        services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
        services.AddScoped<IValidator<ForgotPasswordRequest>, ForgotPasswordRequestValidator>();
        services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordRequestValidator>();
        services.AddScoped<IValidator<AssignPermissionRequest>, AssignPermissionRequestValidator>();
        services.AddScoped<IValidator<CreateCourseRequest>, CreateCourseRequestValidator>();
        services.AddScoped<IValidator<UpdateCourseRequest>, UpdateCourseRequestValidator>();
        services.AddScoped<IValidator<UpdateCourseThumbnailRequest>, UpdateCourseThumbnailRequestValidator>();
        services.AddScoped<IValidator<CreateTeacherRequest>, CreateTeacherRequestValidator>();
        services.AddScoped<IValidator<UpdateTeacherProfileRequest>, UpdateTeacherProfileRequestValidator>();
        services.AddScoped<IValidator<UpdateTeacherContactRequest>, UpdateTeacherContactRequestValidator>();
        services.AddScoped<IValidator<UpdateTeacherProfileImageRequest>, UpdateTeacherProfileImageRequestValidator>();
        services.AddScoped<IValidator<CreateStudentRequest>, CreateStudentRequestValidator>();
        services.AddScoped<IValidator<UpdateStudentProfileRequest>, UpdateStudentProfileRequestValidator>();
        services.AddScoped<IValidator<UpdateStudentContactRequest>, UpdateStudentContactRequestValidator>();
        services.AddScoped<IValidator<UpdateStudentGuardianRequest>, UpdateStudentGuardianRequestValidator>();
        services.AddScoped<IValidator<UpdateStudentProfileImageRequest>, UpdateStudentProfileImageRequestValidator>();
        services.AddScoped<IValidator<CreateBatchRequest>, CreateBatchRequestValidator>();
        services.AddScoped<IValidator<UpdateBatchRequest>, UpdateBatchRequestValidator>();
        services.AddScoped<IValidator<UpdateBatchScheduleRequest>, UpdateBatchScheduleRequestValidator>();
        services.AddScoped<IValidator<UpdateBatchCapacityRequest>, UpdateBatchCapacityRequestValidator>();

        return services;
    }
}
