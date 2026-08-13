using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;
using SchoolERP.Application.Features.ExamWeightSetup.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Manages exam weight setups (e.g. "Mid Term 1 = 20%, Mid Term 2 = 20%,
    /// Half Yearly = 30%, Yearly = 30%") used to compute weighted final
    /// results. Supports versioning (multiple setups per academic year) and
    /// activation (weights must total exactly 100%).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ExamWeightSetupController : ControllerBase
    {
        private readonly IExamWeightSetupService _examWeightSetupService;

        /// <summary>Initializes a new instance of <see cref="ExamWeightSetupController"/>.</summary>
        public ExamWeightSetupController(IExamWeightSetupService examWeightSetupService)
        {
            _examWeightSetupService = examWeightSetupService;
        }

        /// <summary>
        /// Get every exam weight setup.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.WeightSetupView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamWeightSetupDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExamWeightSetupDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a weight setup by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.WeightSetupView)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get every weight setup (active and inactive — version history) for an academic year.
        /// </summary>
        [HttpGet("academic-year/{academicYearId:int}")]
        [PermissionAuthorize(PermissionNames.WeightSetupView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamWeightSetupDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExamWeightSetupDto>>> GetByAcademicYear(int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.GetByAcademicYearAsync(academicYearId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the currently active weight setup for an academic year.
        /// </summary>
        [HttpGet("academic-year/{academicYearId:int}/active")]
        [PermissionAuthorize(PermissionNames.WeightSetupView)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> GetActiveByAcademicYear(int academicYearId, CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.GetActiveByAcademicYearAsync(academicYearId, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new (inactive) weight setup with its items.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExamWeightSetupDto>> Create(
            [FromBody] CreateExamWeightSetupDto request,
            CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Rename an existing weight setup.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> Update(
            int id,
            [FromBody] UpdateExamWeightSetupDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Setup Id must match.");

            var result = await _examWeightSetupService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete an inactive weight setup.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _examWeightSetupService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Activate a setup (requires its items to total exactly 100%). Deactivates any other setup active for the same academic year.
        /// </summary>
        [HttpPost("{id:int}/activate")]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> Activate(int id, CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.ActivateAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Deactivate a setup.
        /// </summary>
        [HttpPost("{id:int}/deactivate")]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> Deactivate(int id, CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.DeactivateAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Add a single exam weight item to an existing (inactive) setup.
        /// </summary>
        [HttpPost("items")]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> AddItem(
            [FromBody] AddExamWeightItemDto request,
            CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.AddItemAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Update a single exam weight item's percentage.
        /// </summary>
        [HttpPut("items/{itemId:int}")]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> UpdateItem(
            int itemId,
            [FromBody] UpdateExamWeightItemDto request,
            CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.UpdateItemAsync(itemId, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Remove a single exam weight item from its setup.
        /// </summary>
        [HttpDelete("items/{itemId:int}")]
        [PermissionAuthorize(PermissionNames.WeightSetupManage)]
        [ProducesResponseType(typeof(ExamWeightSetupDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamWeightSetupDto>> RemoveItem(int itemId, CancellationToken cancellationToken)
        {
            var result = await _examWeightSetupService.RemoveItemAsync(itemId, cancellationToken);
            return Ok(result);
        }
    }
}
