namespace CourseHub.Application.Common.Interfaces;

public interface IRolePermissionRepository
{
    Task<bool> ExistsAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    Task AddAsync(Domain.Entities.RolePermission rolePermission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a single role-permission link, if present. No-op (not an
    /// error) if the link doesn't exist — matching the codebase's
    /// idempotent-mutation style seen elsewhere (e.g. logout).
    /// </summary>
    Task RemoveAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permission names currently assigned to a single role, for the
    /// admin "view role permissions" screen.
    /// </summary>
    Task<IReadOnlyList<string>> GetPermissionNamesForRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Distinct, effective permission names across every role in
    /// <paramref name="roleNames"/> — this is what gets baked into the
    /// JWT access token at login/register/refresh time (see Phase 9:
    /// permission-based authorization).
    /// </summary>
    Task<IReadOnlyList<string>> GetPermissionNamesForRolesAsync(IReadOnlyList<string> roleNames, CancellationToken cancellationToken = default);
}
