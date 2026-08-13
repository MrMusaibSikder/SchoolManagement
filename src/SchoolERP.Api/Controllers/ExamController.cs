using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Exam.DTOs;
using SchoolERP.Application.Features.Exam.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Manages exams: CRUD, the Draft/Published/Completed/Cancelled lifecycle,
    /// the admin dashboard, the exam calendar, and subject-wise routines for
    /// exams/classes/students/teachers. Contains no marks/grade/result
    /// concerns — those belong to the future Result module.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        /// <summary>Initializes a new instance of <see cref="ExamController"/>.</summary>
        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        /// <summary>
        /// Get every exam.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ExamDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _examService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get an exam by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(ExamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get full details for an exam, including every schedule under it.
        /// </summary>
        [HttpGet("{id:int}/details")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(ExamDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDetailsDto>> GetDetails(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.GetExamDetailsAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get scheduling statistics for an exam (subject/class counts, date
        /// span). Contains no marks/results.
        /// </summary>
        [HttpGet("{id:int}/statistics")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(ExamStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamStatisticsDto>> GetStatistics(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.GetExamStatisticsAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Create a new exam as a Draft. AcademicYear + ExamType + Name must be unique.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.ExamCreate)]
        [ProducesResponseType(typeof(ExamDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExamDto>> Create(
            [FromBody] CreateExamDto request,
            CancellationToken cancellationToken)
        {
            var result = await _examService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing exam. Only permitted while the exam is in Draft status.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamEdit)]
        [ProducesResponseType(typeof(ExamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDto>> Update(
            int id,
            [FromBody] UpdateExamDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Exam Id must match.");

            var result = await _examService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete an exam. Not permitted once the exam is Completed or Cancelled.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.ExamDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _examService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Publish a Draft exam.
        /// </summary>
        [HttpPost("{id:int}/publish")]
        [PermissionAuthorize(PermissionNames.ExamPublish)]
        [ProducesResponseType(typeof(ExamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDto>> Publish(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.PublishExamAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Mark a Published exam as Completed.
        /// </summary>
        [HttpPost("{id:int}/complete")]
        [PermissionAuthorize(PermissionNames.ExamComplete)]
        [ProducesResponseType(typeof(ExamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDto>> Complete(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.CompleteExamAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Cancel a Draft or Published exam.
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        [PermissionAuthorize(PermissionNames.ExamCancel)]
        [ProducesResponseType(typeof(ExamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDto>> Cancel(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.CancelExamAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Reopen a Cancelled exam back to Draft.
        /// </summary>
        [HttpPost("{id:int}/reopen")]
        [PermissionAuthorize(PermissionNames.ExamPublish)]
        [ProducesResponseType(typeof(ExamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDto>> Reopen(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.ReopenExamAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get aggregate exam-board statistics and highlights for the admin dashboard.
        /// </summary>
        [HttpGet("dashboard")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(ExamDashboardDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExamDashboardDto>> GetDashboard(CancellationToken cancellationToken)
        {
            var result = await _examService.GetDashboardAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the next upcoming (published, not-yet-occurred) exams.
        /// </summary>
        [HttpGet("upcoming")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(IReadOnlyList<UpcomingExamDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<UpcomingExamDto>>> GetUpcoming(
            [FromQuery] int count = 5,
            CancellationToken cancellationToken = default)
        {
            var result = await _examService.GetUpcomingExamsAsync(count, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every exam schedule within a date range, for a calendar/timetable view.
        /// </summary>
        [HttpGet("calendar")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(IReadOnlyList<ExamCalendarDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<ExamCalendarDto>>> GetCalendar(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate,
            [FromQuery] int? classId,
            CancellationToken cancellationToken)
        {
            var result = await _examService.GetExamCalendarAsync(fromDate, toDate, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the full subject-wise routine for an exam.
        /// </summary>
        [HttpGet("{id:int}/routine")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(ExamRoutineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamRoutineDto>> GetRoutine(int id, CancellationToken cancellationToken)
        {
            var result = await _examService.GetExamRoutineAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the subject-wise routine for a single class within an exam.
        /// </summary>
        [HttpGet("{id:int}/routine/class/{classId:int}")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(ClassRoutineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassRoutineDto>> GetClassRoutine(int id, int classId, CancellationToken cancellationToken)
        {
            var result = await _examService.GetClassRoutineAsync(id, classId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the personal exam routine for a student within an exam.
        /// </summary>
        [HttpGet("{id:int}/routine/student/{studentId:int}")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(StudentRoutineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentRoutineDto>> GetStudentRoutine(int id, int studentId, CancellationToken cancellationToken)
        {
            var result = await _examService.GetStudentRoutineAsync(studentId, id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the routine for a teacher across the subjects they teach, scoped to this exam.
        /// </summary>
        [HttpGet("{id:int}/routine/teacher/{teacherId:int}")]
        [PermissionAuthorize(PermissionNames.ExamView)]
        [ProducesResponseType(typeof(TeacherRoutineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherRoutineDto>> GetTeacherRoutine(int id, int teacherId, CancellationToken cancellationToken)
        {
            var result = await _examService.GetTeacherRoutineAsync(teacherId, id, cancellationToken);
            return Ok(result);
        }
    }
}
