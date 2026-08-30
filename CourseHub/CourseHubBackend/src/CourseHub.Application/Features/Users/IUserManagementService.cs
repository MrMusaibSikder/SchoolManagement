using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Users.Dtos;

namespace CourseHub.Application.Features.Users;

/// <summary>
/// Admin user directory + role assignment. Distinct from
/// IAuthenticationService (which handles the *authenticated* user's own
/// account) and from ITeacherService/IStudentService (which handle
/// promoting a user into a domain profile) — this is purely about
/// managing the User/UserRole rows themselves.
/// </summary>
public interface IUserManagementService
{
    Task<PagedResult<UserResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Idempotent — re-assigning a role the user already has is a no-op,
    /// not an error (mirrors RolePermissionService.AssignPermissionAsync).
    /// Rejects "SuperAdmin" — see AssignRoleRequest.
    /// </summary>
    Task<UserResponse> AssignRoleAsync(Guid userId, AssignRoleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Idempotent — removing a role the user doesn't have is a no-op.
    /// </summary>
    Task<UserResponse> RemoveRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    Task<UserResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UserResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UserResponse> SuspendAsync(Guid id, CancellationToken cancellationToken = default);
}
