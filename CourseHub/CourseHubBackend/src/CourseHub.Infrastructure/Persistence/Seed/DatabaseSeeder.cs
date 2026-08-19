using CourseHub.Application.Common.Options;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CourseHub.Infrastructure.Persistence.Seed;

/// <summary>
/// Idempotent startup seeding: guarantees the default system roles
/// (SuperAdmin/Admin/Teacher/Student, from SeedOptions.DefaultRoles) and
/// the single Institution landing-page profile exist. Safe to run on
/// every startup — each piece only inserts if missing. Run once from
/// Program.cs after the host is built, in a DI scope; not registered as a
/// hosted service to keep it simple and explicit.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(CourseHubDbContext dbContext, SeedOptions seedOptions, CancellationToken cancellationToken = default)
    {
        await SeedRolesAsync(dbContext, seedOptions, cancellationToken);
        await SeedInstitutionAsync(dbContext, seedOptions, cancellationToken);
        await SeedPermissionsAsync(dbContext, seedOptions, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        // Role-Permission links reference Role/Permission rows by Id, and
        // this repository layer resolves those Ids with a fresh database
        // query (not from the in-memory change tracker) — so roles and
        // permissions above must already be persisted before this runs.
        await SeedRolePermissionsAsync(dbContext, seedOptions, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedRolesAsync(
     CourseHubDbContext dbContext,
     SeedOptions seedOptions,
     CancellationToken cancellationToken)
    {
        var existingNames = await dbContext.Roles
            .Select(r => r.Name)
            .ToListAsync(cancellationToken);

        var roleNames = new HashSet<string>(
            existingNames,
            StringComparer.OrdinalIgnoreCase);

        foreach (var roleName in seedOptions.DefaultRoles
                     .Where(name => !string.IsNullOrWhiteSpace(name))
                     .Select(name => name.Trim())
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (roleNames.Contains(roleName))
            {
                continue;
            }

            var role = Role.CreateSystemRole(
                roleName,
                $"Seeded system role: {roleName}");

            await dbContext.Roles.AddAsync(role, cancellationToken);

            // Important: prevent duplicate roles in the same seeding run
            roleNames.Add(roleName);
        }
    }

    private static async Task SeedInstitutionAsync(CourseHubDbContext dbContext, SeedOptions seedOptions, CancellationToken cancellationToken)
    {
        var alreadyExists = await dbContext.Institutions.AnyAsync(cancellationToken);
        if (alreadyExists)
        {
            return;
        }

        var institution = Institution.Create(
            name: seedOptions.InstitutionName,
            slug: seedOptions.InstitutionSlug,
            description: seedOptions.InstitutionDescription,
            isPublic: true);

        await dbContext.Institutions.AddAsync(institution, cancellationToken);
    }

    /// <summary>
    /// Idempotently inserts the global permission catalog (Phase 9).
    /// Mirrors SeedRolesAsync's HashSet-based "insert if missing" shape.
    /// </summary>
    private static async Task SeedPermissionsAsync(
        CourseHubDbContext dbContext,
        SeedOptions seedOptions,
        CancellationToken cancellationToken)
    {
        var existingNames = await dbContext.Permissions
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        var permissionNames = new HashSet<string>(existingNames, StringComparer.OrdinalIgnoreCase);

        foreach (var definition in seedOptions.DefaultPermissions)
        {
            if (permissionNames.Contains(definition.Name))
            {
                continue;
            }

            var permission = Permission.Create(
                definition.Name,
                definition.Resource,
                definition.Action,
                definition.Description);

            await dbContext.Permissions.AddAsync(permission, cancellationToken);

            // Prevent duplicate inserts within the same seeding run.
            permissionNames.Add(definition.Name);
        }
    }

    /// <summary>
    /// Idempotently links each role in SeedOptions.DefaultRolePermissions
    /// to its permissions. Silently skips any role/permission name that
    /// isn't seeded yet instead of throwing, so a partially configured
    /// SeedOptions never blocks application startup.
    /// </summary>
    private static async Task SeedRolePermissionsAsync(
        CourseHubDbContext dbContext,
        SeedOptions seedOptions,
        CancellationToken cancellationToken)
    {
        if (seedOptions.DefaultRolePermissions.Count == 0)
        {
            return;
        }

        var roleIdsByName = await dbContext.Roles
            .ToDictionaryAsync(r => r.Name, r => r.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var permissionIdsByName = await dbContext.Permissions
            .ToDictionaryAsync(p => p.Name, p => p.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingLinks = await dbContext.RolePermissions
            .Select(rp => new { rp.RoleId, rp.PermissionId })
            .ToListAsync(cancellationToken);

        var existingLinkSet = existingLinks
            .Select(link => (link.RoleId, link.PermissionId))
            .ToHashSet();

        foreach (var (roleName, permissionNames) in seedOptions.DefaultRolePermissions)
        {
            if (!roleIdsByName.TryGetValue(roleName, out var roleId))
            {
                continue;
            }

            foreach (var permissionName in permissionNames)
            {
                if (!permissionIdsByName.TryGetValue(permissionName, out var permissionId))
                {
                    continue;
                }

                if (!existingLinkSet.Add((roleId, permissionId)))
                {
                    continue;
                }

                var rolePermission = RolePermission.Create(roleId, permissionId);
                await dbContext.RolePermissions.AddAsync(rolePermission, cancellationToken);
            }
        }
    }
}
