using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Security;
using CourseHub.Application.Features.Users.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Users;

public class UserManagementService : IUserManagementService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserManagementService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<UserResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize <= 0 ? DefaultPageSize : pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _userRepository.SearchAsync(search, normalizedPage, normalizedPageSize, cancellationToken);

        // N+1 role lookups here are acceptable at this scale (single
        // institute, an admin-only screen, page size capped at 100) —
        // not worth a bespoke bulk-join query for this volume.
        var responses = new List<UserResponse>(items.Count);
        foreach (var user in items)
        {
            var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
            responses.Add(ToResponse(user, roles));
        }

        return new PagedResult<UserResponse>(responses, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(id, cancellationToken);
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return ToResponse(user, roles);
    }

    public async Task<UserResponse> AssignRoleAsync(Guid userId, AssignRoleRequest request, CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(userId, cancellationToken);

        if (string.Equals(request.RoleName, SystemRoleNames.SuperAdmin, StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException(
                "SuperAdmin cannot be assigned through role management — it can only be granted via the SuperAdmin invite code during registration.");
        }

        var role = await _roleRepository.GetByNameAsync(request.RoleName, cancellationToken)
            ?? throw new NotFoundException("Role", request.RoleName);

        var alreadyAssigned = await _userRoleRepository.ExistsAsync(user.Id, role.Id, cancellationToken);
        if (!alreadyAssigned)
        {
            var userRole = UserRole.Create(user.Id, role.Id);
            await _userRoleRepository.AddAsync(userRole, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return ToResponse(user, roles);
    }

    public async Task<UserResponse> RemoveRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(userId, cancellationToken);

        var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken)
            ?? throw new NotFoundException("Role", roleName);

        await _userRoleRepository.RemoveAsync(user.Id, role.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return ToResponse(user, roles);
    }

    public async Task<UserResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(id, cancellationToken);
        user.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return ToResponse(user, roles);
    }

    public async Task<UserResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(id, cancellationToken);
        user.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return ToResponse(user, roles);
    }

    public async Task<UserResponse> SuspendAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(id, cancellationToken);
        user.Suspend();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        return ToResponse(user, roles);
    }

    private async Task<User> RequireUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _userRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User", id);
    }

    private static UserResponse ToResponse(User user, IReadOnlyList<string> roles) => new(
        user.Id,
        user.Email,
        user.FirstName,
        user.LastName,
        user.Status.ToString(),
        roles,
        user.LastLoginAt,
        user.CreatedAt);
}
