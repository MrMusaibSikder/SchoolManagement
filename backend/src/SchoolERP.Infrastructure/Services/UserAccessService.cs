using AutoMapper;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Authorization.Interfaces;
using SchoolERP.Application.Features.Permission.DTOs;
using SchoolERP.Application.Features.Role.DTOs;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Resolves a user's effective roles and permissions by combining the
/// UserRole and RolePermission associations already exposed on the Unit of Work.
/// Contains no state of its own; every call re-reads the current data.
/// </summary>
public class UserAccessService : IUserAccessService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserAccessService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoleDto>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _unitOfWork.UserRoleRepository.GetAllAsync(cancellationToken);
        var roleIds = userRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToHashSet();

        if (roleIds.Count == 0)
            return Array.Empty<RoleDto>();

        var allRoles = await _unitOfWork.RoleRepository.GetAllAsync(cancellationToken);
        var roles = allRoles.Where(x => roleIds.Contains(x.Id)).ToList();

        return _mapper.Map<IReadOnlyList<RoleDto>>(roles);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PermissionDto>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _unitOfWork.UserRoleRepository.GetAllAsync(cancellationToken);
        var roleIds = userRoles.Where(x => x.UserId == userId).Select(x => x.RoleId).ToHashSet();

        if (roleIds.Count == 0)
            return Array.Empty<PermissionDto>();

        var rolePermissions = await _unitOfWork.RolePermissionRepository.GetAllAsync(cancellationToken);
        var permissionIds = rolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Select(x => x.PermissionId)
            .ToHashSet();

        if (permissionIds.Count == 0)
            return Array.Empty<PermissionDto>();

        var allPermissions = await _unitOfWork.PermissionRepository.GetAllAsync(cancellationToken);
        var permissions = allPermissions.Where(x => permissionIds.Contains(x.Id)).ToList();

        return _mapper.Map<IReadOnlyList<PermissionDto>>(permissions);
    }

    /// <inheritdoc />
    public async Task<bool> HasPermissionAsync(int userId, string permissionName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(permissionName))
            return false;

        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);

        return permissions.Any(x => string.Equals(x.Name, permissionName, StringComparison.OrdinalIgnoreCase));
    }
}
