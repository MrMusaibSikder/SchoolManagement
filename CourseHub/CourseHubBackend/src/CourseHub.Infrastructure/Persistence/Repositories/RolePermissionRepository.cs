using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Entities;
using CourseHub.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly CourseHubDbContext _dbContext;

    public RolePermissionRepository(CourseHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        return _dbContext.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
    }

    public async Task AddAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
    {
        await _dbContext.RolePermissions.AddAsync(rolePermission, cancellationToken);
    }

    public async Task RemoveAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);

        if (existing is null)
        {
            return;
        }

        _dbContext.RolePermissions.Remove(existing);
    }

    public async Task<IReadOnlyList<string>> GetPermissionNamesForRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var query =
            from rp in _dbContext.RolePermissions
            where rp.RoleId == roleId
            join permission in _dbContext.Permissions on rp.PermissionId equals permission.Id
            select permission.Name;

        return await query.Distinct().ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPermissionNamesForRolesAsync(IReadOnlyList<string> roleNames, CancellationToken cancellationToken = default)
    {
        if (roleNames.Count == 0)
        {
            return Array.Empty<string>();
        }

        // No navigation properties exist between Role/RolePermission/
        // Permission by design (RolePermission is a plain FK join table —
        // see RolePermissionConfiguration), so this joins across DbSets
        // explicitly rather than via .Include().
        var query =
            from role in _dbContext.Roles
            where roleNames.Contains(role.Name)
            join rp in _dbContext.RolePermissions on role.Id equals rp.RoleId
            join permission in _dbContext.Permissions on rp.PermissionId equals permission.Id
            select permission.Name;

        return await query.Distinct().ToListAsync(cancellationToken);
    }
}
