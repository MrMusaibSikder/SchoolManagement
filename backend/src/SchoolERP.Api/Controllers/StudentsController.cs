using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Student.DTOs;
using SchoolERP.Application.Features.Student.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get all students.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.StudentView)]
        [ProducesResponseType(typeof(IReadOnlyList<StudentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<StudentDto>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _studentService.GetAllAsync(cancellationToken));
        }

        /// <summary>
        /// Get student by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.StudentView)]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var student = await _studentService.GetByIdAsync(id, cancellationToken);

            if (student is null)
                return NotFound();

            return Ok(student);
        }

        /// <summary>
        /// Create student.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.StudentCreate)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDto>> Create(
            [FromForm] CreateStudentDto request,
            CancellationToken cancellationToken)
        {
            var student = await _studentService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = student.Id },
                student);
        }

        /// <summary>
        /// Update student.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.StudentEdit)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(StudentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDto>> Update(
            int id,
            [FromForm] UpdateStudentDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Student Id must match.");

            return Ok(await _studentService.UpdateAsync(id, request, cancellationToken));
        }

        /// <summary>
        /// Delete student.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.StudentDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            await _studentService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
