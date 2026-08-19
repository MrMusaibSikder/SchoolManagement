using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Permissions.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Permissions;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RolePermissionService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PermissionResponse>> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        return permissions
            .Select(p => new PermissionResponse(p.Id, p.Name, p.Resource, p.Action, p.Description))
            .ToList();
    }

    public async Task<RolePermissionsResponse> GetPermissionsForRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await RequireRoleAsync(roleId, cancellationToken);
        var permissionNames = await _rolePermissionRepository.GetPermissionNamesForRoleAsync(role.Id, cancellationToken);

        return new RolePermissionsResponse(role.Id, role.Name, permissionNames);
    }

    public async Task<RolePermissionsResponse> AssignPermissionAsync(Guid roleId, AssignPermissionRequest request, CancellationToken cancellationToken = default)
    {
        var role = await RequireRoleAsync(roleId, cancellationToken);

        var permission = await _permissionRepository.GetByNameAsync(request.PermissionName, cancellationToken)
            ?? throw new NotFoundException("Permission", request.PermissionName);

        var alreadyAssigned = await _rolePermissionRepository.ExistsAsync(role.Id, permission.Id, cancellationToken);
        if (!alreadyAssigned)
        {
            var rolePermission = RolePermission.Create(role.Id, permission.Id);
            await _rolePermissionRepository.AddAsync(rolePermission, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var permissionNames = await _rolePermissionRepository.GetPermissionNamesForRoleAsync(role.Id, cancellationToken);
        return new RolePermissionsResponse(role.Id, role.Name, permissionNames);
    }

    public async Task RemovePermissionAsync(Guid roleId, string permissionName, CancellationToken cancellationToken = default)
    {
        var role = await RequireRoleAsync(roleId, cancellationToken);

        var permission = await _permissionRepository.GetByNameAsync(permissionName, cancellationToken)
            ?? throw new NotFoundException("Permission", permissionName);

        await _rolePermissionRepository.RemoveAsync(role.Id, permission.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Role> RequireRoleAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await _roleRepository.GetByIdAsync(roleId, cancellationToken)
            ?? throw new NotFoundException("Role", roleId);
    }
}
