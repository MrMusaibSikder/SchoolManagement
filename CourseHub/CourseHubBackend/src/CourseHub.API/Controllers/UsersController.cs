using CourseHub.API.Security;
using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Users;
using CourseHub.Application.Features.Users.Dtos;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseHub.API.Controllers;

/// <summary>
/// Admin user directory + role assignment. Complements Teachers/Students
/// (which promote a User into a domain profile) and RolePermissions
/// (which manages what a role itself can do) — this manages the User/
/// UserRole rows directly: browsing every account, activating/
/// deactivating/suspending one, and assigning/removing roles.
/// </summary>
[Route("api/admin/users")]
public class UsersController : ApiControllerBase
{
    private readonly IUserManagementService _userManagementService;
    private readonly IValidator<AssignRoleRequest> _assignRoleValidator;

    public UsersController(IUserManagementService userManagementService, IValidator<AssignRoleRequest> assignRoleValidator)
    {
        _userManagementService = userManagementService;
        _assignRoleValidator = assignRoleValidator;
    }

    [HttpGet]
    [HasPermission("users.view")]
    [ProducesResponseType(typeof(PagedResult<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<UserResponse>>> Search(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManagementService.SearchAsync(search, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("users.view")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManagementService.GetByIdAsync(id, cancellationToken);
        return Ok(user);
    }

    /// <summary>
    /// Idempotent — assigning a role the user already has just returns
    /// the current state. Rejects "SuperAdmin" (see AssignRoleRequest).
    /// </summary>
    [HttpPost("{id:guid}/roles")]
    [HasPermission("users.manage")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> AssignRole(Guid id, AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var validationError = await ValidateAsync(_assignRoleValidator, request, cancellationToken);
        if (validationError is not null)
        {
            return validationError;
        }

        var user = await _userManagementService.AssignRoleAsync(id, request, cancellationToken);
        return Ok(user);
    }

    [HttpDelete("{id:guid}/roles/{roleName}")]
    [HasPermission("users.manage")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> RemoveRole(Guid id, string roleName, CancellationToken cancellationToken)
    {
        var user = await _userManagementService.RemoveRoleAsync(id, roleName, cancellationToken);
        return Ok(user);
    }

    [HttpPost("{id:guid}/activate")]
    [HasPermission("users.manage")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Activate(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManagementService.ActivateAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost("{id:guid}/deactivate")]
    [HasPermission("users.manage")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManagementService.DeactivateAsync(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost("{id:guid}/suspend")]
    [HasPermission("users.manage")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Suspend(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userManagementService.SuspendAsync(id, cancellationToken);
        return Ok(user);
    }
}
