using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Subject.DTOs;
using SchoolERP.Application.Features.Subject.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// Get all subjects.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.SubjectView)]
        [ProducesResponseType(typeof(IReadOnlyList<SubjectDto>),StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SubjectDto>>> GetAll(
        CancellationToken cancellationToken)
        {
            var result =await _subjectService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Gets a subject by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.SubjectView)]
        [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubjectDto>> GetById(
            int id,
            CancellationToken cancellationToken)
        {
            var result = await _subjectService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Creates a new subject.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.SubjectCreate)]
        [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SubjectDto>> Create(
            [FromBody] CreateSubjectDto request,
            CancellationToken cancellationToken)
        {
            var result = await _subjectService.CreateAsync(
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// Updates an existing subject.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.SubjectEdit)]
        [ProducesResponseType(typeof(SubjectDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SubjectDto>> Update(
            int id,
            [FromBody] UpdateSubjectDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route id and request id must match.");

            var result = await _subjectService.UpdateAsync(
                id,
                request,
                cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Soft deletes a subject.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.SubjectDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken)
        {
            await _subjectService.DeleteAsync(
                id,
                cancellationToken);

            return NoContent();
        }
    }
}
