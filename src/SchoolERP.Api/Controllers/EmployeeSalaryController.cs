using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.EmployeeSalary.DTOs;
using SchoolERP.Application.Features.EmployeeSalary.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeSalaryController : ControllerBase
    {
        private readonly IEmployeeSalaryService _employeeSalaryService;

        public EmployeeSalaryController(IEmployeeSalaryService employeeSalaryService)
        {
            _employeeSalaryService = employeeSalaryService;
        }

        /// <summary>
        /// Gets all employee salaries.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.EmployeeSalaryView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<EmployeeSalaryDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _employeeSalaryService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets employee salary by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeSalaryView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeSalaryDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _employeeSalaryService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Creates a new employee salary.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.EmployeeSalaryCreate)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EmployeeSalaryDto>> Create(
            CreateEmployeeSalaryDto request,
            CancellationToken cancellationToken)
        {
            var result = await _employeeSalaryService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// Updates an employee salary.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeSalaryUpdate)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmployeeSalaryDto>> Update(
            int id,
            UpdateEmployeeSalaryDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route id and request id do not match.");

            var result = await _employeeSalaryService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Deletes an employee salary.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.EmployeeSalaryDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            await _employeeSalaryService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
