using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.StudentAttendance.DTOs;
using SchoolERP.Application.Features.StudentAttendance.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StudentAttendanceController : ControllerBase
    {
        private readonly IStudentAttendanceService _studentAttendanceService;

        public StudentAttendanceController(
            IStudentAttendanceService studentAttendanceService)
        {
            _studentAttendanceService = studentAttendanceService;
        }

        /// <summary>
        /// Get all student attendance records.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.StudentAttendanceView)]
        [ProducesResponseType(typeof(IReadOnlyList<StudentAttendanceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<StudentAttendanceDto>>> GetAll(
            CancellationToken cancellationToken)
        {
            var result = await _studentAttendanceService.GetAllAsync(cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get attendance by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.StudentAttendanceView)]
        [ProducesResponseType(typeof(StudentAttendanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentAttendanceDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _studentAttendanceService.GetByIdAsync(id, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create attendance for a student.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.StudentAttendanceCreate)]
        [ProducesResponseType(typeof(StudentAttendanceDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<StudentAttendanceDto>> Create(
            [FromBody] CreateStudentAttendanceDto request,
            CancellationToken cancellationToken)
        {
            var result = await _studentAttendanceService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// Take bulk attendance.
        /// </summary>
        [HttpPost("bulk")]
        [PermissionAuthorize(PermissionNames.StudentAttendanceCreate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkAttendance(
            [FromBody] BulkStudentAttendanceDto request,
            CancellationToken cancellationToken)
        {
            await _studentAttendanceService.BulkAttendanceAsync(
                request,
                cancellationToken);

            return Ok(new
            {
                Message = "Attendance saved successfully."
            });
        }

        /// <summary>
        /// Get attendance by Class, Section and Date.
        /// </summary>
        [HttpGet("class-section")]
        [PermissionAuthorize(PermissionNames.StudentAttendanceView)]
        [ProducesResponseType(typeof(IReadOnlyList<StudentAttendanceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<StudentAttendanceDto>>> GetByClassSectionDate(
            [FromQuery] int classId,
            [FromQuery] int sectionId,
            [FromQuery] DateTime attendanceDate,
            CancellationToken cancellationToken)
        {
            var result =
                await _studentAttendanceService.GetByClassSectionDateAsync(
                    classId,
                    sectionId,
                    attendanceDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get attendance history of a student.
        /// </summary>
        [HttpGet("student/{studentId:int}/history")]
        [PermissionAuthorize(PermissionNames.StudentAttendanceView)]
        [ProducesResponseType(typeof(IReadOnlyList<StudentAttendanceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<StudentAttendanceDto>>> GetStudentHistory(
            int studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result =
                await _studentAttendanceService.GetStudentHistoryAsync(
                    studentId,
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Update attendance.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.StudentAttendanceEdit)]
        [ProducesResponseType(typeof(StudentAttendanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentAttendanceDto>> Update(
            int id,
            [FromBody] UpdateStudentAttendanceDto request,
            CancellationToken cancellationToken)
        {
            
            var result =
                await _studentAttendanceService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Delete attendance.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.StudentAttendanceDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            await _studentAttendanceService.DeleteAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }
}
