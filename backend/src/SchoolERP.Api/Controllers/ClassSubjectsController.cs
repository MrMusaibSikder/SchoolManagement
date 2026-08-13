using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.ClassSubject.DTOs;
using SchoolERP.Application.Features.ClassSubject.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ClassSubjectsController : ControllerBase
    {
        private readonly IClassSubjectService _classSubjectService;

        public ClassSubjectsController(IClassSubjectService classSubjectService)
        {
            _classSubjectService = classSubjectService;
        }

        /// <summary>
        /// Get all class-subject mappings.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.ClassSubjectView)]
        [ProducesResponseType(typeof(IReadOnlyList<ClassSubjectDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ClassSubjectDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var result = await _classSubjectService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific class-subject mapping.
        /// </summary>
        [HttpGet("{classId:int}/{subjectId:int}")]
        [PermissionAuthorize(PermissionNames.ClassSubjectView)]
        [ProducesResponseType(typeof(ClassSubjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassSubjectDto>> Get(
            int classId,
            int subjectId,
            CancellationToken cancellationToken = default)
        {
            var result = await _classSubjectService.GetAsync(
                classId,
                subjectId,
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Assign a subject to a class.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.ClassSubjectAssign)]
        [ProducesResponseType(typeof(ClassSubjectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ClassSubjectDto>> Assign(
            [FromBody] ClassSubjectDto request,
            CancellationToken cancellationToken = default)
        {
            var result = await _classSubjectService.AssignAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new
                {
                    classId = result.ClassId,
                    subjectId = result.SubjectId
                },
                result);
        }

        /// <summary>
        /// Remove a subject from a class.
        /// </summary>
        [HttpDelete("{classId:int}/{subjectId:int}")]
        [PermissionAuthorize(PermissionNames.ClassSubjectRemove)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Remove(
            int classId,
            int subjectId,
            CancellationToken cancellationToken = default)
        {
            await _classSubjectService.RemoveAsync(
                classId,
                subjectId,
                cancellationToken);

            return NoContent();
        }
        /// <summary>
        /// Mark a subject as optional (e.g. Higher Math, Agriculture, ICT
        /// Practical) or mandatory for a class. Optional subjects never count
        /// toward a student's fail count and contribute only a bonus grade
        /// point toward the final GPA.
        /// </summary>
        [HttpPatch("{classId:int}/{subjectId:int}/optional")]
        [PermissionAuthorize(PermissionNames.ClassSubjectAssign)]
        [ProducesResponseType(typeof(ClassSubjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassSubjectDto>> SetOptional(
            int classId,
            int subjectId,
            [FromQuery] bool isOptional,
            CancellationToken cancellationToken = default)
        {
            var result = await _classSubjectService.SetOptionalAsync(classId, subjectId, isOptional, cancellationToken);
            return Ok(result);
        }
    }
}
