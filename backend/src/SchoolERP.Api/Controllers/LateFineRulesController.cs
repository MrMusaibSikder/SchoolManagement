using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.LateFineRule.DTOs;
using SchoolERP.Application.Features.LateFineRule.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class LateFineRulesController : ControllerBase
    {
        private readonly ILateFineRuleService _lateFineRuleService;

        public LateFineRulesController(ILateFineRuleService lateFineRuleService)
        {
            _lateFineRuleService = lateFineRuleService;
        }

        /// <summary>Get all late fine rules for an academic year.</summary>
        [HttpGet("academic-year/{academicYearId:int}")]
        [PermissionAuthorize(PermissionNames.LateFineRuleView)]
        [ProducesResponseType(typeof(IReadOnlyList<LateFineRuleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<LateFineRuleDto>>> GetByAcademicYear(
            int academicYearId, CancellationToken cancellationToken)
        {
            return Ok(await _lateFineRuleService.GetByAcademicYearAsync(academicYearId, cancellationToken));
        }

        /// <summary>Create a late fine rule (either global, or specific to a fee type).</summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.LateFineRuleManage)]
        [ProducesResponseType(typeof(LateFineRuleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LateFineRuleDto>> Create(
            [FromBody] CreateLateFineRuleDto request, CancellationToken cancellationToken)
        {
            var rule = await _lateFineRuleService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetByAcademicYear), new { academicYearId = rule.AcademicYearId }, rule);
        }

        /// <summary>Update a late fine rule.</summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.LateFineRuleManage)]
        [ProducesResponseType(typeof(LateFineRuleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LateFineRuleDto>> Update(
            int id, [FromBody] UpdateLateFineRuleDto request, CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and body Id must match.");
            return Ok(await _lateFineRuleService.UpdateAsync(id, request, cancellationToken));
        }

        /// <summary>Soft-delete a late fine rule.</summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.LateFineRuleManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _lateFineRuleService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
