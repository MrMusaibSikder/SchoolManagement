using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Permission.DTOs;
using SchoolERP.Application.Features.Permission.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers;

/// <summary>Admin endpoints for managing Permissions (CRUD).</summary>
[ApiController]
[Route("api/[controller]")]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    /// <summary>Initializes a new instance of <see cref="PermissionController"/>.</summary>
    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    /// <summary>Gets every permission.</summary>
    [HttpGet]
    [PermissionAuthorize(PermissionNames.PermissionView)]
    [ProducesResponseType(typeof(IReadOnlyList<PermissionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var permissions = await _permissionService.GetAllAsync(cancellationToken);
        return Ok(permissions);
    }

    /// <summary>Gets a single permission by id.</summary>
    [HttpGet("{id:int}")]
    [PermissionAuthorize(PermissionNames.PermissionView)]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PermissionDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var permission = await _permissionService.GetByIdAsync(id, cancellationToken);
        return permission is null ? NotFound() : Ok(permission);
    }

    /// <summary>Creates a new permission.</summary>
    [HttpPost]
    [PermissionAuthorize(PermissionNames.PermissionCreate)]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PermissionDto>> Create([FromBody] CreatePermissionDto request, CancellationToken cancellationToken)
    {
        var permission = await _permissionService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
    }

    /// <summary>Updates an existing permission.</summary>
    [HttpPut("{id:int}")]
    [PermissionAuthorize(PermissionNames.PermissionEdit)]
    [ProducesResponseType(typeof(PermissionDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PermissionDto>> Update(int id, [FromBody] UpdatePermissionDto request, CancellationToken cancellationToken)
    {
        var permission = await _permissionService.UpdateAsync(id, request, cancellationToken);
        return Ok(permission);
    }

    /// <summary>Deletes (soft-deletes) an existing permission.</summary>
    [HttpDelete("{id:int}")]
    [PermissionAuthorize(PermissionNames.PermissionDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _permissionService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
