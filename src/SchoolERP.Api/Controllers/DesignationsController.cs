using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Designation.DTOs;
using SchoolERP.Application.Features.Designation.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DesignationsController : ControllerBase
    {
        private readonly IDesignationService _designationService;

        public DesignationsController(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        /// <summary>
        /// Get all designations.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.DesignationView)]
        [ProducesResponseType(typeof(IReadOnlyList<DesignationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<DesignationDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var designations = await _designationService.GetAllAsync(cancellationToken);
            return Ok(designations);
        }

        /// <summary>
        /// Get designation by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.DesignationView)]
        [ProducesResponseType(typeof(DesignationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DesignationDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var designation = await _designationService.GetByIdAsync(id, cancellationToken);

            if (designation is null)
                return NotFound();

            return Ok(designation);
        }

        /// <summary>
        /// Create designation.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.DesignationCreate)]
        [ProducesResponseType(typeof(DesignationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DesignationDto>> Create(
            [FromBody] CreateDesignationDto request,
            CancellationToken cancellationToken = default)
        {
            var designation = await _designationService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = designation.Id },
                designation);
        }

        /// <summary>
        /// Update designation.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.DesignationEdit)]
        [ProducesResponseType(typeof(DesignationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DesignationDto>> Update(
            int id,
            [FromBody] UpdateDesignationDto request,
            CancellationToken cancellationToken = default)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Designation Id must match.");

            var designation = await _designationService.UpdateAsync(id, request, cancellationToken);

            return Ok(designation);
        }

        /// <summary>
        /// Delete designation.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.DesignationDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            
            await _designationService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
