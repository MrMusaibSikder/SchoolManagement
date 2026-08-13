using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.School.DTOs;
using SchoolERP.Application.Features.School.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SchoolController : ControllerBase
    {

        private readonly ISchoolService _schoolService;

        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        /// <summary>
        /// Get all schools.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.SchoolView)]
        [ProducesResponseType(typeof(IReadOnlyList<SchoolDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SchoolDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _schoolService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get school by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.SchoolView)]
        [ProducesResponseType(typeof(SchoolDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SchoolDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _schoolService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new school.
        /// </summary>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [PermissionAuthorize(PermissionNames.SchoolCreate)]
        [ProducesResponseType(typeof(SchoolDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SchoolDto>> Create(
            [FromForm] CreateSchoolDto request,
            CancellationToken cancellationToken)
        {
            var result = await _schoolService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        /// <summary>
        /// Update an existing school.
        /// </summary>
        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [PermissionAuthorize(PermissionNames.SchoolUpdate)]
        [ProducesResponseType(typeof(SchoolDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SchoolDto>> Update(
            int id,
            [FromForm] UpdateSchoolDto request,
            CancellationToken cancellationToken)
        {
            var result = await _schoolService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete a school.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.SchoolDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _schoolService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}