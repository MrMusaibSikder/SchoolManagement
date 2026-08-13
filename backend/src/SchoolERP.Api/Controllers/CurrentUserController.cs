using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Features.Authentication.DTOs;
using SchoolERP.Application.Features.Authorization.Interfaces;
using SchoolERP.Application.Features.Permission.DTOs;
using SchoolERP.Application.Features.Role.DTOs;
using SchoolERP.Application.Features.User.Interfaces;

namespace SchoolERP.Api.Controllers;

/// <summary>
/// Endpoints describing the currently authenticated user: profile, roles and
/// effective permissions, derived from the validated JWT rather than from a
/// route parameter, so a user can never query anyone else's data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CurrentUserController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly IUserAccessService _userAccessService;

    /// <summary>Initializes a new instance of <see cref="CurrentUserController"/>.</summary>
    public CurrentUserController(
        ICurrentUserService currentUserService,
        IUserService userService,
        IUserAccessService userAccessService)
    {
        _currentUserService = currentUserService;
        _userService = userService;
        _userAccessService = userAccessService;
    }

    /// <summary>Gets the current user's profile, roles and effective permissions.</summary>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(CurrentUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentUserDto>> GetProfile(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is not { } userId)
            return Unauthorized();

        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return NotFound();

        var roles = await _userAccessService.GetUserRolesAsync(userId, cancellationToken);
        var permissions = await _userAccessService.GetUserPermissionsAsync(userId, cancellationToken);

        return Ok(new CurrentUserDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            Roles = roles.Select(x => x.Name).ToList(),
            Permissions = permissions.Select(x => x.Name).ToList()
        });
    }

    /// <summary>Gets the roles assigned to the current user.</summary>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(IReadOnlyList<RoleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetRoles(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is not { } userId)
            return Unauthorized();

        var roles = await _userAccessService.GetUserRolesAsync(userId, cancellationToken);
        return Ok(roles);
    }

    /// <summary>Gets the effective permissions granted to the current user.</summary>
    [HttpGet("permissions")]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetPermissions(CancellationToken cancellationToken)
    {
        if (_currentUserService.UserId is not { } userId)
            return Unauthorized();

        var permissions = await _userAccessService.GetUserPermissionsAsync(userId, cancellationToken);
        return Ok(permissions);
    }
}
