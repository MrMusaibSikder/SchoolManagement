using CourseHub.Application.Features.Auth.Dtos;
using CourseHub.Application.Features.Batches.Dtos;
using CourseHub.Application.Features.Courses.Dtos;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Application.Features.Permissions.Dtos;
using CourseHub.Application.Features.Students.Dtos;
using CourseHub.Application.Features.Teachers.Dtos;
using CourseHub.Application.Features.Users.Dtos;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CourseHub.API.Swagger;

/// <summary>
/// Phase 13: attaches a realistic example JSON body to the request DTOs
/// most people will actually try first in Swagger UI. Written by hand
/// against Swashbuckle's existing extensibility points (IOperationFilter
/// + Microsoft.OpenApi.Any) rather than a separate examples NuGet
/// package — both are already transitive dependencies of
/// Swashbuckle.AspNetCore, so this needed no new package reference.
///
/// Deliberately NOT exhaustive — every "Create"/"Update" DTO across every
/// controller could get one, but the highest-value targets are covered:
/// the full auth lifecycle, and one representative create/update per
/// admin resource. Response bodies aren't given custom examples; Swagger's
/// default schema-shape example (auto-generated from each DTO's C# types)
/// is good enough there — request bodies are what people actually have to
/// type/paste correctly to get a 2xx.
/// </summary>
public class RequestExampleOperationFilter : IOperationFilter
{
    private static readonly Dictionary<Type, IOpenApiAny> Examples = new()
    {
        [typeof(RegisterRequest)] = new OpenApiObject
        {
            ["email"] = new OpenApiString("rahim.uddin@example.com"),
            ["password"] = new OpenApiString("P@ssw0rd123!"),
            ["confirmPassword"] = new OpenApiString("P@ssw0rd123!"),
            ["firstName"] = new OpenApiString("Rahim"),
            ["lastName"] = new OpenApiString("Uddin"),
            ["requestedRole"] = new OpenApiString("Student"),
            ["superAdminCode"] = new OpenApiNull(),
        },
        [typeof(LoginRequest)] = new OpenApiObject
        {
            ["email"] = new OpenApiString("rahim.uddin@example.com"),
            ["password"] = new OpenApiString("P@ssw0rd123!"),
        },
        [typeof(ChangePasswordRequest)] = new OpenApiObject
        {
            ["currentPassword"] = new OpenApiString("P@ssw0rd123!"),
            ["newPassword"] = new OpenApiString("EvenStr0ngerP@ss!"),
            ["confirmNewPassword"] = new OpenApiString("EvenStr0ngerP@ss!"),
        },
        [typeof(ForgotPasswordRequest)] = new OpenApiObject
        {
            ["email"] = new OpenApiString("rahim.uddin@example.com"),
        },
        [typeof(ResetPasswordRequest)] = new OpenApiObject
        {
            ["token"] = new OpenApiString("the-token-emailed-to-you"),
            ["newPassword"] = new OpenApiString("EvenStr0ngerP@ss!"),
            ["confirmNewPassword"] = new OpenApiString("EvenStr0ngerP@ss!"),
        },
        [typeof(AssignPermissionRequest)] = new OpenApiObject
        {
            ["permissionName"] = new OpenApiString("courses.view"),
        },
        [typeof(AssignRoleRequest)] = new OpenApiObject
        {
            ["roleName"] = new OpenApiString("Teacher"),
        },
        [typeof(CreateCourseRequest)] = new OpenApiObject
        {
            ["name"] = new OpenApiString("Full-Stack Web Development"),
            ["code"] = new OpenApiString("FSWD-01"),
            ["durationInMonths"] = new OpenApiInteger(6),
            ["description"] = new OpenApiString("From HTML/CSS fundamentals to a deployed full-stack app."),
        },
        [typeof(UpdateCourseRequest)] = new OpenApiObject
        {
            ["name"] = new OpenApiString("Full-Stack Web Development (Updated)"),
            ["code"] = new OpenApiString("FSWD-01"),
            ["durationInMonths"] = new OpenApiInteger(7),
            ["description"] = new OpenApiString("From HTML/CSS fundamentals to a deployed full-stack app."),
        },
        [typeof(UpdateCourseThumbnailRequest)] = new OpenApiObject
        {
            ["thumbnailUrl"] = new OpenApiString("https://cdn.example.com/courses/fswd-01.png"),
        },
        [typeof(CreateTeacherRequest)] = new OpenApiObject
        {
            ["userId"] = new OpenApiString("3f1a2b4c-7d8e-4a1b-9c2d-1e2f3a4b5c6d"),
            ["employeeId"] = new OpenApiString("EMP-001"),
            ["firstName"] = new OpenApiString("Rahim"),
            ["lastName"] = new OpenApiString("Uddin"),
        },
        [typeof(UpdateTeacherProfileRequest)] = new OpenApiObject
        {
            ["firstName"] = new OpenApiString("Rahim"),
            ["lastName"] = new OpenApiString("Uddin"),
            ["bio"] = new OpenApiString("Full-stack instructor with 6 years of industry experience."),
        },
        [typeof(UpdateTeacherContactRequest)] = new OpenApiObject
        {
            ["phone"] = new OpenApiString("+8801XXXXXXXXX"),
            ["email"] = new OpenApiString("rahim.teaches@example.com"),
        },
        [typeof(CreateStudentRequest)] = new OpenApiObject
        {
            ["userId"] = new OpenApiString("6a7b8c9d-1e2f-4a3b-8c4d-5e6f7a8b9c0d"),
            ["studentId"] = new OpenApiString("STU-2026-001"),
            ["firstName"] = new OpenApiString("Karim"),
            ["lastName"] = new OpenApiString("Islam"),
        },
        [typeof(UpdateStudentProfileRequest)] = new OpenApiObject
        {
            ["firstName"] = new OpenApiString("Karim"),
            ["lastName"] = new OpenApiString("Islam"),
            ["dateOfBirth"] = new OpenApiString("2005-03-14"),
        },
        [typeof(UpdateStudentGuardianRequest)] = new OpenApiObject
        {
            ["guardianName"] = new OpenApiString("Abdul Islam"),
            ["guardianPhone"] = new OpenApiString("+8801XXXXXXXXX"),
        },
        [typeof(CreateBatchRequest)] = new OpenApiObject
        {
            ["courseId"] = new OpenApiString("9f8e7d6c-5b4a-3c2d-1e0f-a1b2c3d4e5f6"),
            ["name"] = new OpenApiString("Morning Batch — Jan 2027"),
            ["code"] = new OpenApiString("FSWD-01-B3"),
            ["startDate"] = new OpenApiString("2027-01-15"),
            ["capacity"] = new OpenApiInteger(25),
        },
        [typeof(UpdateBatchScheduleRequest)] = new OpenApiObject
        {
            ["startDate"] = new OpenApiString("2027-01-15"),
            ["endDate"] = new OpenApiString("2027-07-15"),
        },
        [typeof(CreateEnrollmentRequest)] = new OpenApiObject
        {
            ["studentId"] = new OpenApiString("2c3d4e5f-6a7b-4c8d-9e0f-1a2b3c4d5e6f"),
            ["batchId"] = new OpenApiString("9f8e7d6c-5b4a-3c2d-1e0f-a1b2c3d4e5f6"),
        },
    };

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.RequestBody?.Content is null)
        {
            return;
        }

        var bodyParameterType = context.ApiDescription.ParameterDescriptions
            .FirstOrDefault(p => p.Source.Id == "Body")?.Type;

        if (bodyParameterType is null || !Examples.TryGetValue(bodyParameterType, out var example))
        {
            return;
        }

        foreach (var content in operation.RequestBody.Content.Values)
        {
            content.Example = example;
        }
    }
}
