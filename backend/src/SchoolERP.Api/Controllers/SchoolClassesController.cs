using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.SchoolClass.DTOs;
using SchoolERP.Application.Features.SchoolClass.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolClassesController : ControllerBase
    {
        private readonly ISchoolClassService _schoolClassService;

        public SchoolClassesController(ISchoolClassService schoolClassService)
        {
            _schoolClassService = schoolClassService;
        }

        /// <summary>
        /// Get all classes.
        /// Musaib sikder
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.SchoolClassView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SchoolClassDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _schoolClassService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get class by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.SchoolClassView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SchoolClassDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _schoolClassService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create new class.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.SchoolClassCreate)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SchoolClassDto>> Create(
            [FromBody] CreateSchoolClassDto request,
            CancellationToken cancellationToken)
        {
            var result = await _schoolClassService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update class.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.SchoolClassEdit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SchoolClassDto>> Update(
            int id,
            [FromBody] UpdateSchoolClassDto request,
            CancellationToken cancellationToken)
        {
            var result = await _schoolClassService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete class.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.SchoolClassDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _schoolClassService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
