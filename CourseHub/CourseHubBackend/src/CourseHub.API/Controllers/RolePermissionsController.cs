using CourseHub.API.Security;
using CourseHub.Application.Features.Permissions;
using CourseHub.Application.Features.Permissions.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Phase 9 admin surface: browse the global permission catalog and manage
/// which permissions a role has. Every action requires "roles.manage" (or
/// "roles.view"/"permissions.view" for read-only endpoints) — SuperAdmin
/// always passes via PermissionAuthorizationHandler's bypass, so this
/// stays usable even before any RolePermission rows exist yet.
/// </summary>
[ApiController]
[Route("api/admin")]
public class RolePermissionsController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;
    private readonly IValidator<AssignPermissionRequest> _assignPermissionValidator;

    public RolePermissionsController(
        IRolePermissionService rolePermissionService,
        IValidator<AssignPermissionRequest> assignPermissionValidator)
    {
        _rolePermissionService = rolePermissionService;
        _assignPermissionValidator = assignPermissionValidator;
    }

    [HttpGet("permissions")]
    [HasPermission("permissions.view")]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<PermissionResponse>>> GetCatalog(CancellationToken cancellationToken)
    {
        var catalog = await _rolePermissionService.GetCatalogAsync(cancellationToken);
        return Ok(catalog);
    }

    [HttpGet("roles/{roleId:guid}/permissions")]
    [HasPermission("roles.view")]
    [ProducesResponseType(typeof(RolePermissionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RolePermissionsResponse>> GetRolePermissions(Guid roleId, CancellationToken cancellationToken)
    {
        var response = await _rolePermissionService.GetPermissionsForRoleAsync(roleId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("roles/{roleId:guid}/permissions")]
    [HasPermission("roles.manage")]
    [ProducesResponseType(typeof(RolePermissionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RolePermissionsResponse>> AssignPermission(Guid roleId, AssignPermissionRequest request, CancellationToken cancellationToken)
    {
        var validation = await _assignPermissionValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            foreach (var error in validation.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var response = await _rolePermissionService.AssignPermissionAsync(roleId, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("roles/{roleId:guid}/permissions/{permissionName}")]
    [HasPermission("roles.manage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemovePermission(Guid roleId, string permissionName, CancellationToken cancellationToken)
    {
        await _rolePermissionService.RemovePermissionAsync(roleId, permissionName, cancellationToken);
        return NoContent();
    }
}
