using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.FeeCategory.DTOs;
using SchoolERP.Application.Features.FeeCategory.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FeeCategoriesController : ControllerBase
    {
        private readonly IFeeCategoryService _feeCategoryService;

        public FeeCategoriesController(IFeeCategoryService feeCategoryService)
        {
            _feeCategoryService = feeCategoryService;
        }

        /// <summary>Get all active fee categories, ordered by display order.</summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.FeeCategoryView)]
        [ProducesResponseType(typeof(IReadOnlyList<FeeCategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<FeeCategoryDto>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _feeCategoryService.GetAllAsync(cancellationToken));
        }

        /// <summary>Get fee category by Id.</summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeCategoryView)]
        [ProducesResponseType(typeof(FeeCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeeCategoryDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var category = await _feeCategoryService.GetByIdAsync(id, cancellationToken);
            if (category is null)
                return NotFound();
            return Ok(category);
        }

        /// <summary>Create fee category.</summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.FeeCategoryCreate)]
        [ProducesResponseType(typeof(FeeCategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FeeCategoryDto>> Create(
            [FromBody] CreateFeeCategoryDto request,
            CancellationToken cancellationToken)
        {
            var category = await _feeCategoryService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>Update fee category.</summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeCategoryEdit)]
        [ProducesResponseType(typeof(FeeCategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeeCategoryDto>> Update(
            int id, [FromBody] UpdateFeeCategoryDto request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and body Id must match.");
            return Ok(await _feeCategoryService.UpdateAsync(id, request, cancellationToken));
        }

        /// <summary>Soft-delete fee category.</summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeCategoryDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _feeCategoryService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
