using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Permission.Interfaces;
using SchoolERP.Application.Features.Role.DTOs;
using SchoolERP.Application.Features.Role.Interfaces;
using SchoolERP.Application.Features.RolePermission.DTOs;
using SchoolERP.Application.Features.RolePermission.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers;

/// <summary>
/// Admin endpoints for managing Roles: CRUD plus assigning permissions to a role.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;
    private readonly IRolePermissionService _rolePermissionService;

    /// <summary>Initializes a new instance of <see cref="RoleController"/>.</summary>
    public RoleController(
        IRoleService roleService,
        IPermissionService permissionService,
        IRolePermissionService rolePermissionService)
    {
        _roleService = roleService;
        _permissionService = permissionService;
        _rolePermissionService = rolePermissionService;
    }

    /// <summary>Gets every role.</summary>
    [HttpGet]
    [PermissionAuthorize(PermissionNames.RoleView)]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(cancellationToken);
        return Ok(roles);
    }

    /// <summary>Gets a single role by id.</summary>
    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionNames.RoleView)]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RoleDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var role = await _roleService.GetByIdAsync(id, cancellationToken);
        return role is null ? NotFound() : Ok(role);
    }

    /// <summary>Creates a new role.</summary>
    [HttpPost]
    [PermissionAuthorize(PermissionNames.RoleCreate)]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto request, CancellationToken cancellationToken)
    {
        var role = await _roleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    /// <summary>Updates an existing role.</summary>
    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionNames.RoleEdit)]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RoleDto>> Update(int id, [FromBody] UpdateRoleDto request, CancellationToken cancellationToken)
    {
        var role = await _roleService.UpdateAsync(id, request, cancellationToken);
        return Ok(role);
    }

    /// <summary>Deletes (soft-deletes) an existing role.</summary>
    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionNames.RoleDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _roleService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Gets every permission currently assigned to the given role.</summary>
    [HttpGet("{id:int}/permissions")]
    [PermissionAuthorize(PermissionNames.RoleView)]
    [ProducesResponseType(typeof(IReadOnlyList<Application.Features.Permission.DTOs.PermissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<Application.Features.Permission.DTOs.PermissionDto>>> GetPermissions(
        int id, CancellationToken cancellationToken)
    {
        var rolePermissions = await _rolePermissionService.GetAllAsync(cancellationToken);
        var permissionIds = rolePermissions.Where(x => x.RoleId == id).Select(x => x.PermissionId).ToHashSet();

        var allPermissions = await _permissionService.GetAllAsync(cancellationToken);
        var permissions = allPermissions.Where(x => permissionIds.Contains(x.Id)).ToList();

        return Ok(permissions);
    }

    /// <summary>
    /// Assigns a batch of permissions to a role in one call. Permissions already
    /// assigned to the role are skipped rather than causing an error.
    /// </summary>
    [HttpPost("assign-permissions")]
    [PermissionAuthorize(PermissionNames.RoleAssignPermission)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionsToRoleDto request, CancellationToken cancellationToken)
    {
        foreach (var permissionId in request.PermissionIds.Distinct())
        {
            var existing = await _rolePermissionService.GetAsync(request.RoleId, permissionId, cancellationToken);
            if (existing is null)
            {
                await _rolePermissionService.AssignAsync(
                    new RolePermissionDto { RoleId = request.RoleId, PermissionId = permissionId },
                    cancellationToken);
            }
        }

        return NoContent();
    }

    /// <summary>Removes a single permission from a role.</summary>
    [HttpDelete("{roleId:int}/permissions/{permissionId:int}")]
    [PermissionAuthorize(PermissionNames.RoleAssignPermission)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemovePermission(int roleId, int permissionId, CancellationToken cancellationToken)
    {
        await _rolePermissionService.RemoveAsync(roleId, permissionId, cancellationToken);
        return NoContent();
    }
}
