using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.SubjectTeacher.DTOs;
using SchoolERP.Application.Features.SubjectTeacher.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SubjectTeachersController : ControllerBase
    {
        
       

            private readonly ISubjectTeacherService _subjectTeacherService;

            public SubjectTeachersController(ISubjectTeacherService subjectTeacherService)
            {
                _subjectTeacherService = subjectTeacherService;
            }

            /// <summary>
            /// Get all subject-teacher assignments.
            /// </summary>
            [HttpGet]
            [PermissionAuthorize(PermissionNames.SubjectTeacherView)]
            [ProducesResponseType(typeof(IReadOnlyList<SubjectTeacherDto>), StatusCodes.Status200OK)]
            public async Task<ActionResult<IReadOnlyList<SubjectTeacherDto>>> GetAll(
                CancellationToken cancellationToken = default)
            {
                var result = await _subjectTeacherService.GetAllAsync(cancellationToken);
                return Ok(result);
            }

            /// <summary>
            /// Get a specific subject-teacher assignment.
            /// </summary>
            [HttpGet("{subjectId:int}/{teacherId:int}")]
            [PermissionAuthorize(PermissionNames.SubjectTeacherView)]
            [ProducesResponseType(typeof(SubjectTeacherDto), StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<ActionResult<SubjectTeacherDto>> Get(
                int subjectId,
                int teacherId,
                CancellationToken cancellationToken = default)
            {
                var result = await _subjectTeacherService.GetAsync(
                    subjectId,
                    teacherId,
                    cancellationToken);

                if (result is null)
                    return NotFound();

                return Ok(result);
            }

            /// <summary>
            /// Assign a subject to a teacher.
            /// </summary>
            [HttpPost]
            [PermissionAuthorize(PermissionNames.SubjectTeacherAssign)]
            [ProducesResponseType(typeof(SubjectTeacherDto), StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<ActionResult<SubjectTeacherDto>> Assign(
                [FromBody] SubjectTeacherDto request,
                CancellationToken cancellationToken = default)
            {
                var result = await _subjectTeacherService.AssignAsync(
                    request,
                    cancellationToken);

                return CreatedAtAction(
                    nameof(Get),
                    new
                    {
                        subjectId = result.SubjectId,
                        teacherId = result.TeacherId
                    },
                    result);
            }

            /// <summary>
            /// Remove a subject-teacher assignment.
            /// </summary>
            [HttpDelete("{subjectId:int}/{teacherId:int}")]
            [PermissionAuthorize(PermissionNames.SubjectTeacherRemove)]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Remove(
                int subjectId,
                int teacherId,
                CancellationToken cancellationToken = default)
            {
                await _subjectTeacherService.RemoveAsync(
                    subjectId,
                    teacherId,
                    cancellationToken);

                return NoContent();
            }
        
    }
}
