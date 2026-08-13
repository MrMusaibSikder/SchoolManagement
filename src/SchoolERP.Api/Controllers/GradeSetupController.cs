using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.GradeSetup.DTOs;
using SchoolERP.Application.Features.GradeSetup.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Manages configurable grade bands (e.g. A+, A, A-, B, C, D, F) per
    /// academic year, used by every result calculation instead of a hardcoded
    /// scale — grading policy can change year to year without a code change.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GradeSetupController : ControllerBase
    {
        private readonly IGradeSetupService _gradeSetupService;

        /// <summary>Initializes a new instance of <see cref="GradeSetupController"/>.</summary>
        public GradeSetupController(IGradeSetupService gradeSetupService)
        {
            _gradeSetupService = gradeSetupService;
        }

        /// <summary>
        /// Get every grade band.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.GradeSetupView)]
        [ProducesResponseType(typeof(IReadOnlyList<GradeSetupDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<GradeSetupDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _gradeSetupService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a grade band by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.GradeSetupView)]
        [ProducesResponseType(typeof(GradeSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeSetupDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _gradeSetupService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get every grade band configured for an academic year, ordered by display order.
        /// </summary>
        [HttpGet("academic-year/{academicYearId:int}")]
        [PermissionAuthorize(PermissionNames.GradeSetupView)]
        [ProducesResponseType(typeof(IReadOnlyList<GradeSetupDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<GradeSetupDto>>> GetByAcademicYear(int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _gradeSetupService.GetByAcademicYearAsync(academicYearId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a new grade band. Its percentage range must not overlap any other active band in the same academic year.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.GradeSetupManage)]
        [ProducesResponseType(typeof(GradeSetupDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GradeSetupDto>> Create(
            [FromBody] CreateGradeSetupDto request,
            CancellationToken cancellationToken)
        {
            var result = await _gradeSetupService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing grade band. Its percentage range must not overlap any other active band in the same academic year.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.GradeSetupManage)]
        [ProducesResponseType(typeof(GradeSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeSetupDto>> Update(
            int id,
            [FromBody] UpdateGradeSetupDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Grade Setup Id must match.");

            var result = await _gradeSetupService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Activate a grade band.
        /// </summary>
        [HttpPost("{id:int}/activate")]
        [PermissionAuthorize(PermissionNames.GradeSetupManage)]
        [ProducesResponseType(typeof(GradeSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeSetupDto>> Activate(int id, CancellationToken cancellationToken)
        {
            var result = await _gradeSetupService.ActivateAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Deactivate a grade band without deleting it.
        /// </summary>
        [HttpPost("{id:int}/deactivate")]
        [PermissionAuthorize(PermissionNames.GradeSetupManage)]
        [ProducesResponseType(typeof(GradeSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GradeSetupDto>> Deactivate(int id, CancellationToken cancellationToken)
        {
            var result = await _gradeSetupService.DeactivateAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete a grade band.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.GradeSetupManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _gradeSetupService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
