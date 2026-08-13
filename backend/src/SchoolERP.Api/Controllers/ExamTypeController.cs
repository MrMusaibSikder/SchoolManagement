using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.ExamType.DTOs;
using SchoolERP.Application.Features.ExamType.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Manages exam type categories (e.g. Term, Half-Yearly, Final) used to
    /// classify exams.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ExamTypeController : ControllerBase
    {
        private readonly IExamTypeService _examTypeService;

        /// <summary>Initializes a new instance of <see cref="ExamTypeController"/>.</summary>
        public ExamTypeController(IExamTypeService examTypeService)
        {
            _examTypeService = examTypeService;
        }

        /// <summary>
        /// Get every exam type.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.ExamTypeView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamTypeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExamTypeDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _examTypeService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get an exam type by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamTypeView)]
        [ProducesResponseType(typeof(ExamTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamTypeDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _examTypeService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new exam type. Names must be unique.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.ExamTypeCreate)]
        [ProducesResponseType(typeof(ExamTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExamTypeDto>> Create(
            [FromBody] CreateExamTypeDto request,
            CancellationToken cancellationToken)
        {
            var result = await _examTypeService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing exam type. Names must be unique.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamTypeEdit)]
        [ProducesResponseType(typeof(ExamTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamTypeDto>> Update(
            int id,
            [FromBody] UpdateExamTypeDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Exam Type Id must match.");

            var result = await _examTypeService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete an exam type. Not permitted while any exam still uses it.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamTypeDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _examTypeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
