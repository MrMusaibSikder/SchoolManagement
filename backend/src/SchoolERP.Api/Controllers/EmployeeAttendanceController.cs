using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.EmployeeAttendance.DTOs;
using SchoolERP.Application.Features.EmployeeAttendance.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Endpoints for recording and querying staff (employee) attendance.
    /// Mirrors the <see cref="StudentAttendanceController"/> architecture.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EmployeeAttendanceController : ControllerBase
    {
        private readonly IEmployeeAttendanceService _employeeAttendanceService;

        /// <summary>Initializes a new instance of <see cref="EmployeeAttendanceController"/>.</summary>
        public EmployeeAttendanceController(
            IEmployeeAttendanceService employeeAttendanceService)
        {
            _employeeAttendanceService = employeeAttendanceService;
        }

        /// <summary>
        /// Get all employee attendance records.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceView)]
        [ProducesResponseType(typeof(IReadOnlyList<EmployeeAttendanceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EmployeeAttendanceDto>>> GetAll(
            CancellationToken cancellationToken)
        {
            var result = await _employeeAttendanceService.GetAllAsync(cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get attendance by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceView)]
        [ProducesResponseType(typeof(EmployeeAttendanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeAttendanceDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _employeeAttendanceService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create attendance for a single employee.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceCreate)]
        [ProducesResponseType(typeof(EmployeeAttendanceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeAttendanceDto>> Create(
            [FromBody] CreateEmployeeAttendanceDto request,
            CancellationToken cancellationToken)
        {
            var result = await _employeeAttendanceService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// Take bulk (staff-wide) attendance for a single date.
        /// </summary>
        [HttpPost("bulk")]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceCreate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> BulkAttendance(
            [FromBody] BulkEmployeeAttendanceDto request,
            CancellationToken cancellationToken)
        {
            await _employeeAttendanceService.BulkAttendanceAsync(
                request,
                cancellationToken);

            return Ok(new
            {
                Message = "Attendance saved successfully."
            });
        }

        /// <summary>
        /// Get every employee's attendance for a specific date.
        /// </summary>
        [HttpGet("by-date")]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceView)]
        [ProducesResponseType(typeof(IReadOnlyList<EmployeeAttendanceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EmployeeAttendanceDto>>> GetByDate(
            [FromQuery] DateTime attendanceDate,
            CancellationToken cancellationToken)
        {
            var result = await _employeeAttendanceService.GetByDateAsync(attendanceDate, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get attendance history of an employee.
        /// </summary>
        [HttpGet("employee/{employeeId:int}/history")]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceView)]
        [ProducesResponseType(typeof(IReadOnlyList<EmployeeAttendanceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EmployeeAttendanceDto>>> GetEmployeeHistory(
            int employeeId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            CancellationToken cancellationToken)
        {
            var result =
                await _employeeAttendanceService.GetEmployeeHistoryAsync(
                    employeeId,
                    fromDate,
                    toDate,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Update attendance.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceEdit)]
        [ProducesResponseType(typeof(EmployeeAttendanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeAttendanceDto>> Update(
            int id,
            [FromBody] UpdateEmployeeAttendanceDto request,
            CancellationToken cancellationToken)
        {
            var result =
                await _employeeAttendanceService.UpdateAsync(
                    id,
                    request,
                    cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Delete attendance.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeAttendanceDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            await _employeeAttendanceService.DeleteAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }
}
