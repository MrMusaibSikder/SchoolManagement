namespace CourseHub.Application.Common.Options;

/// <summary>
/// Strongly typed seeding configuration, bound from "Seed".
/// SuperAdminInviteCode must come from User Secrets (dev) or environment
/// variables/secret manager (production) — never from appsettings.json.
/// It is the "raw code" a person supplies at registration to be granted
/// the SuperAdmin role; anyone who doesn't know it just gets the Student
/// role by default (or Teacher, if explicitly requested).
/// </summary>
public class SeedOptions
{
    public const string SectionName = "Seed";

    public string SuperAdminInviteCode { get; set; } = string.Empty;

    /// <summary>
    /// Roles guaranteed to exist after startup seeding. SuperAdmin/Admin/
    /// Teacher/Student are seeded as IsSystemRole=true (protected); an
    /// admin can still create further custom roles dynamically later.
    /// </summary>
    public string[] DefaultRoles { get; set; } =
    {
        Security.SystemRoleNames.SuperAdmin,
        Security.SystemRoleNames.Admin,
        Security.SystemRoleNames.Teacher,
        Security.SystemRoleNames.Student,
    };

    /// <summary>
    /// Default single-institute profile, seeded once if no Institution row
    /// exists yet, so the public landing page has data out of the box.
    /// Contains no secrets — safe to keep in appsettings.json.
    /// </summary>
    public string InstitutionName { get; set; } = "CourseHub Institute";

    public string InstitutionSlug { get; set; } = "coursehub";

    public string? InstitutionDescription { get; set; }

    /// <summary>
    /// Global permission catalog guaranteed to exist after startup
    /// seeding (Phase 9). Each entry is idempotently inserted by name —
    /// safe to append new permissions here in later phases (e.g. Phase 12
    /// will add "courses.create", "students.manage", etc.) without
    /// touching existing rows.
    /// </summary>
    public IReadOnlyList<PermissionSeedDefinition> DefaultPermissions { get; set; } = new List<PermissionSeedDefinition>
    {
        new("roles.manage", "Role", "Manage", "Create/update roles and assign or remove permissions on them."),
        new("roles.view", "Role", "View", "View the list of roles and which permissions each one has."),
        new("permissions.view", "Permission", "View", "View the global permission catalog."),
    };

    /// <summary>
    /// Default role -> permission-name assignments for every role EXCEPT
    /// SuperAdmin. SuperAdmin is deliberately not configured here — it is
    /// auto-granted every permission in the catalog directly by
    /// DatabaseSeeder.SeedRolePermissionsAsync on every startup, so it
    /// never drifts out of sync as new permissions are added in later
    /// phases and its assignments stay visible as real RolePermission
    /// rows (not just a runtime bypass).
    /// </summary>
    public IReadOnlyDictionary<string, string[]> DefaultRolePermissions { get; set; } =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [Security.SystemRoleNames.Admin] = new[] { "roles.manage", "roles.view", "permissions.view" },
        };
}

/// <summary>
/// One row of the seeded permission catalog. A plain record (not a Domain
/// entity) because this only exists to drive idempotent seeding — see
/// DatabaseSeeder.SeedPermissionsAsync.
/// </summary>
public record PermissionSeedDefinition(string Name, string Resource, string Action, string? Description = null);
