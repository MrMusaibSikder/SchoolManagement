using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using SchoolERP.Application.Features.FeeStructure.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>Manages class-wise, academic-year-wise fee structures (rate cards).</summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FeeStructuresController : ControllerBase
    {
        private readonly IFeeStructureService _feeStructureService;

        public FeeStructuresController(IFeeStructureService feeStructureService)
        {
            _feeStructureService = feeStructureService;
        }

        /// <summary>Get fee structures, filtered by academic year / class / active status.</summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.FeeStructureView)]
        [ProducesResponseType(typeof(IReadOnlyList<FeeStructureListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<FeeStructureListDto>>> GetList(
            [FromQuery] int? academicYearId,
            [FromQuery] int? schoolClassId,
            [FromQuery] bool? isActive,
            CancellationToken cancellationToken)
        {
            return Ok(await _feeStructureService.GetListAsync(academicYearId, schoolClassId, isActive, cancellationToken));
        }

        /// <summary>Get fee structure by Id, with all items.</summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeStructureView)]
        [ProducesResponseType(typeof(FeeStructureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeeStructureDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var structure = await _feeStructureService.GetByIdAsync(id, cancellationToken);
            if (structure is null)
                return NotFound();
            return Ok(structure);
        }

        /// <summary>Create fee structure with items.</summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.FeeStructureCreate)]
        [ProducesResponseType(typeof(FeeStructureDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FeeStructureDto>> Create(
            [FromBody] CreateFeeStructureDto request, CancellationToken cancellationToken)
        {
            var structure = await _feeStructureService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = structure.Id }, structure);
        }

        /// <summary>Update fee structure, including item add/update/soft-delete merge.</summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeStructureEdit)]
        [ProducesResponseType(typeof(FeeStructureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FeeStructureDto>> Update(
            int id, [FromBody] UpdateFeeStructureDto request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and body Id must match.");
            return Ok(await _feeStructureService.UpdateAsync(id, request, cancellationToken));
        }

        /// <summary>Soft-delete fee structure (only allowed if no invoices reference it).</summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.FeeStructureDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _feeStructureService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}