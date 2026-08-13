using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Section.DTOs;
using SchoolERP.Application.Features.Section.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionsController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        /// <summary>
        /// Get all sections.
        /// Musaib Sikder
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.SectionView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SectionDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _sectionService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get section by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.SectionView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _sectionService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create new section.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.SectionCreate)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SectionDto>> Create(
            [FromBody] CreateSectionDto request,
            CancellationToken cancellationToken)
        {
            var result = await _sectionService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update section.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.SectionEdit)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SectionDto>> Update(
            int id,
            [FromBody] UpdateSectionDto request,
            CancellationToken cancellationToken)
        {
            var result = await _sectionService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete section.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.SectionDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _sectionService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
