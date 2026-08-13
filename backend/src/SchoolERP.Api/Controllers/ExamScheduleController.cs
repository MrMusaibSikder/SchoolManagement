using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.ExamSchedule.DTOs;
using SchoolERP.Application.Features.ExamSchedule.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Manages subject-wise exam schedules: date, class, subject and marking
    /// scheme for each exam. Schedule changes are gated by the parent exam's
    /// lifecycle status (see <see cref="SchoolERP.Domain.Enums.ExamStatus"/>).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ExamScheduleController : ControllerBase
    {
        private readonly IExamScheduleService _examScheduleService;

        /// <summary>Initializes a new instance of <see cref="ExamScheduleController"/>.</summary>
        public ExamScheduleController(IExamScheduleService examScheduleService)
        {
            _examScheduleService = examScheduleService;
        }

        /// <summary>
        /// Get every exam schedule.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.ExamScheduleView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamScheduleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExamScheduleDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _examScheduleService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get an exam schedule by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamScheduleView)]
        [ProducesResponseType(typeof(ExamScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamScheduleDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _examScheduleService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get every schedule for a given exam, ordered by exam date.
        /// </summary>
        [HttpGet("exam/{examId:int}")]
        [PermissionAuthorize(PermissionNames.ExamScheduleView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ExamScheduleDto>>> GetByExam(int examId, CancellationToken cancellationToken)
        {
            var result = await _examScheduleService.GetSchedulesByExamAsync(examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every schedule for a given class, optionally restricted to a single exam.
        /// </summary>
        [HttpGet("class/{classId:int}")]
        [PermissionAuthorize(PermissionNames.ExamScheduleView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ExamScheduleDto>>> GetByClass(
            int classId,
            [FromQuery] int? examId,
            CancellationToken cancellationToken)
        {
            var result = await _examScheduleService.GetSchedulesByClassAsync(classId, examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every schedule for subjects taught by a given teacher, optionally restricted to a single exam.
        /// </summary>
        [HttpGet("teacher/{teacherId:int}")]
        [PermissionAuthorize(PermissionNames.ExamScheduleView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<ExamScheduleDto>>> GetByTeacher(
            int teacherId,
            [FromQuery] int? examId,
            CancellationToken cancellationToken)
        {
            var result = await _examScheduleService.GetSchedulesByTeacherAsync(teacherId, examId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a new exam schedule. The parent exam must not be Completed or
        /// Cancelled; the subject and date must not already be taken for the
        /// same exam and class.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.ExamScheduleCreate)]
        [ProducesResponseType(typeof(ExamScheduleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamScheduleDto>> Create(
            [FromBody] CreateExamScheduleDto request,
            CancellationToken cancellationToken)
        {
            var result = await _examScheduleService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing exam schedule. Not permitted once the parent exam
        /// is Completed or Cancelled.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamScheduleEdit)]
        [ProducesResponseType(typeof(ExamScheduleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamScheduleDto>> Update(
            int id,
            [FromBody] UpdateExamScheduleDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Exam Schedule Id must match.");

            var result = await _examScheduleService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete an exam schedule. Not permitted once the parent exam is
        /// Completed or Cancelled.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamScheduleDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _examScheduleService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
