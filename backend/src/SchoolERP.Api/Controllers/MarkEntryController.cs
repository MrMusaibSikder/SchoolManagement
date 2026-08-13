using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Result.DTOs;
using SchoolERP.Application.Features.Result.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Subject-level mark entry: create/update single marks, bulk class entry,
    /// draft/submit workflow, and admin lock/unlock. Enforces that the
    /// requesting teacher is assigned to the subject (via the existing
    /// SubjectTeacher mapping) and that the parent exam is Published.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MarkEntryController : ControllerBase
    {
        private readonly IResultService _resultService;

        /// <summary>Initializes a new instance of <see cref="MarkEntryController"/>.</summary>
        public MarkEntryController(IResultService resultService)
        {
            _resultService = resultService;
        }

        /// <summary>
        /// Get every mark entry.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.MarksEntryView)]
        [ProducesResponseType(typeof(IReadOnlyList<ResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _resultService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a mark entry by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.MarksEntryView)]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get every mark entry for a given exam schedule (one subject, one class).
        /// </summary>
        [HttpGet("exam-schedule/{examScheduleId:int}")]
        [PermissionAuthorize(PermissionNames.MarksEntryView)]
        [ProducesResponseType(typeof(IReadOnlyList<ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> GetByExamSchedule(int examScheduleId, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetByExamScheduleAsync(examScheduleId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every mark entry for a student across every subject of one exam.
        /// </summary>
        [HttpGet("student/{studentId:int}/exam/{examId:int}")]
        [PermissionAuthorize(PermissionNames.MarksEntryView)]
        [ProducesResponseType(typeof(IReadOnlyList<ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> GetByStudentAndExam(int studentId, int examId, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetByStudentAndExamAsync(studentId, examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Enter a single student's mark as Draft. The teacher must be assigned
        /// to the schedule's subject and the exam must be Published.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.MarksEntryCreate)]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultDto>> Create(
            [FromBody] CreateResultDto request,
            CancellationToken cancellationToken)
        {
            var result = await _resultService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing, unlocked mark entry.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.MarksEntryEdit)]
        [ProducesResponseType(typeof(ResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResultDto>> Update(
            int id,
            [FromBody] UpdateResultDto request,
            CancellationToken cancellationToken)
        {
            var result = await _resultService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Enter or update marks for an entire class in one call (upsert per student).
        /// </summary>
        [HttpPost("bulk")]
        [PermissionAuthorize(PermissionNames.MarksEntryCreate)]
        [ProducesResponseType(typeof(IReadOnlyList<ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> BulkEntry(
            [FromBody] BulkMarkEntryDto request,
            CancellationToken cancellationToken)
        {
            var result = await _resultService.BulkEntryAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Finalize every Draft mark entry for an exam schedule, moving them to Submitted.
        /// </summary>
        [HttpPost("exam-schedule/{examScheduleId:int}/submit")]
        [PermissionAuthorize(PermissionNames.MarksEntryEdit)]
        [ProducesResponseType(typeof(IReadOnlyList<ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ResultDto>>> Submit(
            int examScheduleId,
            [FromQuery] int teacherId,
            CancellationToken cancellationToken)
        {
            var result = await _resultService.SubmitAsync(examScheduleId, teacherId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Admin-only: lock every mark entry for an exam schedule, preventing further edits.
        /// </summary>
        [HttpPost("exam-schedule/{examScheduleId:int}/lock")]
        [PermissionAuthorize(PermissionNames.MarksEntryPublish)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Lock(int examScheduleId, CancellationToken cancellationToken)
        {
            await _resultService.LockByExamScheduleAsync(examScheduleId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Admin-only: unlock every mark entry for an exam schedule so corrections can be made.
        /// </summary>
        [HttpPost("exam-schedule/{examScheduleId:int}/unlock")]
        [PermissionAuthorize(PermissionNames.MarksEntryPublish)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Unlock(int examScheduleId, CancellationToken cancellationToken)
        {
            await _resultService.UnlockByExamScheduleAsync(examScheduleId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Delete an unlocked mark entry.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.MarksEntryDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _resultService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
