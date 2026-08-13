using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Teacher.DTOs;
using SchoolERP.Application.Features.Teacher.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        /// <summary>
        /// Get all teachers.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.TeacherView)]
        [ProducesResponseType(typeof(IReadOnlyList<TeacherDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TeacherDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var teachers = await _teacherService.GetAllAsync(cancellationToken);
            return Ok(teachers);
        }

        /// <summary>
        /// Get teacher by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.TeacherView)]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var teacher = await _teacherService.GetByIdAsync(id, cancellationToken);

            if (teacher is null)
                return NotFound();

            return Ok(teacher);
        }

        /// <summary>
        /// Create teacher.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.TeacherCreate)]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TeacherDto>> Create(
            [FromBody] CreateTeacherDto request,
            CancellationToken cancellationToken = default)
        {
            var teacher = await _teacherService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = teacher.Id },
                teacher);
        }

        /// <summary>
        /// Update teacher.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.TeacherEdit)]
        [ProducesResponseType(typeof(TeacherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDto>> Update(
            int id,
            [FromBody] UpdateTeacherDto request,
            CancellationToken cancellationToken = default)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Teacher Id must match.");

            var teacher = await _teacherService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return Ok(teacher);
        }

        /// <summary>
        /// Delete teacher.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.TeacherDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            await _teacherService.DeleteAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }
}

