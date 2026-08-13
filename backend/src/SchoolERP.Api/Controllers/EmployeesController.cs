using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Employee.DTOs;
using SchoolERP.Application.Features.Employee.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Get all employees.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.EmployeeView)]
        [ProducesResponseType(typeof(IReadOnlyList<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EmployeeDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var employees = await _employeeService.GetAllAsync(cancellationToken);
            return Ok(employees);
        }

        /// <summary>
        /// Get employee by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeView)]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var employee = await _employeeService.GetByIdAsync(id, cancellationToken);

            if (employee is null)
                return NotFound();

            return Ok(employee);
        }

        /// <summary>
        /// Create employee.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.EmployeeCreate)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeDto>> Create(
            [FromForm] CreateEmployeeDto request,
            CancellationToken cancellationToken = default)
        {
            var employee = await _employeeService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.Id },
                employee);
        }

        /// <summary>
        /// Update employee.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeEdit)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeDto>> Update(
            int id,
            [FromForm] UpdateEmployeeDto request,
            CancellationToken cancellationToken = default)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Employee Id must match.");

            var employee = await _employeeService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return Ok(employee);
        }

        /// <summary>
        /// Delete employee.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            await _employeeService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
