using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.SmsTemplate.DTOs;
using SchoolERP.Application.Features.SmsTemplate.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Manages reusable SMS message templates, including placeholder-based
    /// rendering. Actual SMS delivery is out of scope for this controller; it
    /// only prepares the message content that a future SMS gateway integration
    /// will send.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class SmsTemplateController : ControllerBase
    {
        private readonly ISmsTemplateService _smsTemplateService;

        /// <summary>Initializes a new instance of <see cref="SmsTemplateController"/>.</summary>
        public SmsTemplateController(ISmsTemplateService smsTemplateService)
        {
            _smsTemplateService = smsTemplateService;
        }

        /// <summary>
        /// Get a search-filtered, paged, sorted list of SMS templates.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.SmsTemplateView)]
        [ProducesResponseType(typeof(PagedResult<SmsTemplateDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<SmsTemplateDto>>> GetPaged(
            [FromQuery] SmsTemplateQueryDto query,
            CancellationToken cancellationToken)
        {
            var result = await _smsTemplateService.GetPagedAsync(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every SMS template, without paging. Useful for populating
        /// dropdowns where the full list is needed.
        /// </summary>
        [HttpGet("all")]
        [PermissionAuthorize(PermissionNames.SmsTemplateView)]
        [ProducesResponseType(typeof(IReadOnlyList<SmsTemplateDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<SmsTemplateDto>>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _smsTemplateService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get an SMS template by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.SmsTemplateView)]
        [ProducesResponseType(typeof(SmsTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SmsTemplateDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _smsTemplateService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new SMS template. Template names must be unique.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.SmsTemplateCreate)]
        [ProducesResponseType(typeof(SmsTemplateDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SmsTemplateDto>> Create(
            [FromBody] CreateSmsTemplateDto request,
            CancellationToken cancellationToken)
        {
            var result = await _smsTemplateService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing SMS template. Template names must be unique.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.SmsTemplateEdit)]
        [ProducesResponseType(typeof(SmsTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SmsTemplateDto>> Update(
            int id,
            [FromBody] UpdateSmsTemplateDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Template Id must match.");

            var result = await _smsTemplateService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Activate a template, making it available for use.
        /// </summary>
        [HttpPatch("{id:int}/activate")]
        [PermissionAuthorize(PermissionNames.SmsTemplateEdit)]
        [ProducesResponseType(typeof(SmsTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SmsTemplateDto>> Activate(int id, CancellationToken cancellationToken)
        {
            var result = await _smsTemplateService.ActivateAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Deactivate a template without deleting it.
        /// </summary>
        [HttpPatch("{id:int}/deactivate")]
        [PermissionAuthorize(PermissionNames.SmsTemplateEdit)]
        [ProducesResponseType(typeof(SmsTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SmsTemplateDto>> Deactivate(int id, CancellationToken cancellationToken)
        {
            var result = await _smsTemplateService.DeactivateAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Preview a template rendered with real placeholder values, without
        /// sending anything. Useful for verifying the final SMS text before an
        /// SMS gateway integration is wired up.
        /// </summary>
        [HttpPost("{id:int}/preview")]
        [PermissionAuthorize(PermissionNames.SmsTemplateView)]
        [ProducesResponseType(typeof(RenderedSmsTemplateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RenderedSmsTemplateDto>> Preview(
            int id,
            [FromBody] RenderSmsTemplateDto request,
            CancellationToken cancellationToken)
        {
            var result = await _smsTemplateService.RenderAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete (soft-delete) an SMS template.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.SmsTemplateDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _smsTemplateService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
