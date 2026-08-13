using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Authorization.Interfaces;
using SchoolERP.Application.Features.Permission.DTOs;
using SchoolERP.Application.Features.Role.DTOs;
using SchoolERP.Application.Features.User.DTOs;
using SchoolERP.Application.Features.User.Interfaces;
using SchoolERP.Application.Features.UserRole.DTOs;
using SchoolERP.Application.Features.UserRole.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers;

/// <summary>
/// Admin endpoints for managing Users: CRUD, plus assigning/removing roles and
/// reading a user's effective roles and permissions.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly IUserAccessService _userAccessService;

    /// <summary>Initializes a new instance of <see cref="UserController"/>.</summary>
    public UserController(
        IUserService userService,
        IUserRoleService userRoleService,
        IUserAccessService userAccessService)
    {
        _userService = userService;
        _userRoleService = userRoleService;
        _userAccessService = userAccessService;
    }

    /// <summary>Gets every user.</summary>
    [HttpGet]
    [PermissionAuthorize(PermissionNames.UserView)]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    /// <summary>Gets a single user by id.</summary>
    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionNames.UserView)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    /// <summary>Creates a new user.</summary>
    [HttpPost]
    [PermissionAuthorize(PermissionNames.UserCreate)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto request, CancellationToken cancellationToken)
    {
        var user = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>Updates an existing user.</summary>
    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionNames.UserEdit)]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UpdateUserDto request, CancellationToken cancellationToken)
    {
        var user = await _userService.UpdateAsync(id, request, cancellationToken);
        return Ok(user);
    }

    /// <summary>Deletes (soft-deletes) an existing user.</summary>
    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionNames.UserDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    /// <summary>Gets every role currently assigned to the given user.</summary>
    [HttpGet("{id:int}/roles")]
    [PermissionAuthorize(PermissionNames.UserRoleView)]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetRoles(int id, CancellationToken cancellationToken)
    {
        var roles = await _userAccessService.GetUserRolesAsync(id, cancellationToken);
        return Ok(roles);
    }

    /// <summary>
    /// Gets the distinct, effective set of permissions granted to the given user
    /// through all of their assigned roles.
    /// </summary>
    [HttpGet("{id:int}/permissions")]
    [PermissionAuthorize(PermissionNames.UserRoleView)]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetPermissions(int id, CancellationToken cancellationToken)
    {
        var permissions = await _userAccessService.GetUserPermissionsAsync(id, cancellationToken);
        return Ok(permissions);
    }

    /// <summary>Assigns a role to a user.</summary>
    [HttpPost("assign-role")]
    [PermissionAuthorize(PermissionNames.UserRoleAssign)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleToUserDto request, CancellationToken cancellationToken)
    {
        var existing = await _userRoleService.GetAsync(request.UserId, request.RoleId, cancellationToken);
        if (existing is null)
        {
            await _userRoleService.AssignAsync(
                new UserRoleDto { UserId = request.UserId, RoleId = request.RoleId },
                cancellationToken);
        }

        return NoContent();
    }

    /// <summary>Removes a role from a user.</summary>
    [HttpDelete("{userId:int}/roles/{roleId:int}")]
    [PermissionAuthorize(PermissionNames.UserRoleRemove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RemoveRole(int userId, int roleId, CancellationToken cancellationToken)
    {
        await _userRoleService.RemoveAsync(userId, roleId, cancellationToken);
        return NoContent();
    }
}
