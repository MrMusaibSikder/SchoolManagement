using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Guardian.DTOs;
using SchoolERP.Application.Features.Guardian.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class GuardiansController : ControllerBase
    {
        private readonly IGuardianService _guardianService;

        public GuardiansController(IGuardianService guardianService)
        {
            _guardianService = guardianService;
        }


        /// <summary>
        /// Get all guardians.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.GuardianView)]
        [ProducesResponseType(typeof(IReadOnlyList<GuardianDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<GuardianDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var guardians = await _guardianService.GetAllAsync(cancellationToken);

            return Ok(guardians);
        }


        /// <summary>
        /// Get guardian by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.GuardianView)]
        [ProducesResponseType(typeof(GuardianDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GuardianDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var guardian = await _guardianService.GetByIdAsync(id, cancellationToken);

            if (guardian is null)
                return NotFound();

            return Ok(guardian);
        }


        /// <summary>
        /// Search guardians by name or phone number.
        /// </summary>
        [HttpGet("search")]
        [PermissionAuthorize(PermissionNames.GuardianView)]
        [ProducesResponseType(typeof(IReadOnlyList<GuardianDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<GuardianDto>>> Search(
            [FromQuery] string keyword,
            CancellationToken cancellationToken = default)
        {
            var guardians = await _guardianService.SearchAsync(
                keyword,
                cancellationToken);

            return Ok(guardians);
        }


        /// <summary>
        /// Create guardian.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.GuardianCreate)]
        [ProducesResponseType(typeof(GuardianDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GuardianDto>> Create(
            [FromBody] CreateGuardianDto request,
            CancellationToken cancellationToken = default)
        {
            var guardian = await _guardianService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = guardian.Id },
                guardian);
        }


        /// <summary>
        /// Update guardian.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.GuardianEdit)]
        [ProducesResponseType(typeof(GuardianDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GuardianDto>> Update(
            int id,
            [FromBody] UpdateGuardianDto request,
            CancellationToken cancellationToken = default)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Guardian Id must match.");

            var guardian = await _guardianService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return Ok(guardian);
        }


        /// <summary>
        /// Delete guardian.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.GuardianDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            await _guardianService.DeleteAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }
}
