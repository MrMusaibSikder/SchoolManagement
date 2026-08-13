using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.FeeType.DTOs;
using SchoolERP.Application.Features.FeeType.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FeeTypesController : ControllerBase
    {
        private readonly IFeeTypeService _feeTypeService;

        public FeeTypesController(IFeeTypeService feeTypeService)
        {
            _feeTypeService = feeTypeService;
        }

        /// <summary>Get all fee types (lightweight list, with category name).</summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.FeeTypeView)]
        [ProducesResponseType(typeof(IReadOnlyList<FeeTypeListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<FeeTypeListDto>>> GetAll(CancellationToken cancellationToken)
        {
            return Ok(await _feeTypeService.GetAllAsync(cancellationToken));
        }

        /// <summary>Get fee type by Id (full detail).</summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeTypeView)]
        [ProducesResponseType(typeof(FeeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeeTypeDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var feeType = await _feeTypeService.GetByIdAsync(id, cancellationToken);
            if (feeType is null)
                return NotFound();
            return Ok(feeType);
        }

        /// <summary>Create fee type.</summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.FeeTypeCreate)]
        [ProducesResponseType(typeof(FeeTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FeeTypeDto>> Create(
            [FromBody] CreateFeeTypeDto request, CancellationToken cancellationToken)
        {
            var feeType = await _feeTypeService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = feeType.Id }, feeType);
        }

        /// <summary>Update fee type.</summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeTypeEdit)]
        [ProducesResponseType(typeof(FeeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeeTypeDto>> Update(
            int id, [FromBody] UpdateFeeTypeDto request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and body Id must match.");
            return Ok(await _feeTypeService.UpdateAsync(id, request, cancellationToken));
        }

        /// <summary>Soft-delete fee type.</summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeTypeDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _feeTypeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
