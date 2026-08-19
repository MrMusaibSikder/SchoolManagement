namespace CourseHub.Application.Common.Security;

/// <summary>
/// Names of the seeded, protected system roles (see SeedOptions.DefaultRoles
/// and Role.CreateSystemRole). Centralized so both Application (e.g.
/// registration role resolution) and API (e.g. the SuperAdmin permission
/// bypass in PermissionAuthorizationHandler) reference the same string
/// instead of redeclaring it independently.
/// </summary>
public static class SystemRoleNames
{
    public const string SuperAdmin = "SuperAdmin";

    public const string Admin = "Admin";

    public const string Teacher = "Teacher";

    public const string Student = "Student";
}
