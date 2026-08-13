using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.Notice.DTOs;
using SchoolERP.Application.Features.Notice.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    /// <summary>
    /// Manages the school notice board: draft/publish/archive/restore workflow,
    /// audience/priority targeting, optional attachments, and dashboard
    /// statistics. SMS/Email delivery for notices flagged with
    /// <c>SendSms</c>/<c>SendEmail</c> is architected for but not yet performed
    /// here (future gateway/email integration).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class NoticeController : ControllerBase
    {
        private readonly INoticeService _noticeService;

        /// <summary>Initializes a new instance of <see cref="NoticeController"/>.</summary>
        public NoticeController(INoticeService noticeService)
        {
            _noticeService = noticeService;
        }

        /// <summary>
        /// Get a search-filtered, paged, sorted list of notices. Supports
        /// filtering by audience, priority, publish/archive state and publish
        /// date range.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(PagedResult<NoticeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<NoticeDto>>> GetPaged(
            [FromQuery] NoticeQueryDto query,
            CancellationToken cancellationToken)
        {
            var result = await _noticeService.GetPagedAsync(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get a notice by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _noticeService.GetByIdAsync(id, cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Create a new notice as a draft. Titles must be unique. Accepts an
        /// optional attachment file.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.NoticeCreate)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NoticeDto>> Create(
            [FromForm] CreateNoticeDto request,
            CancellationToken cancellationToken)
        {
            var result = await _noticeService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing notice. Titles must be unique. Does not change the
        /// publish/archive state — use the dedicated publish/unpublish/archive/
        /// restore endpoints for that.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.NoticeEdit)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> Update(
            int id,
            [FromForm] UpdateNoticeDto request,
            CancellationToken cancellationToken)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Notice Id must match.");

            var result = await _noticeService.UpdateAsync(id, request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Publish a draft notice, making it visible to its target audience.
        /// </summary>
        [HttpPost("{id:int}/publish")]
        [PermissionAuthorize(PermissionNames.NoticePublish)]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> Publish(int id, CancellationToken cancellationToken)
        {
            var result = await _noticeService.PublishAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Revert a published notice back to draft state.
        /// </summary>
        [HttpPost("{id:int}/unpublish")]
        [PermissionAuthorize(PermissionNames.NoticePublish)]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> Unpublish(int id, CancellationToken cancellationToken)
        {
            var result = await _noticeService.UnpublishAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Archive a notice, retiring it from active circulation.
        /// </summary>
        [HttpPost("{id:int}/archive")]
        [PermissionAuthorize(PermissionNames.NoticePublish)]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> Archive(int id, CancellationToken cancellationToken)
        {
            var result = await _noticeService.ArchiveAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Restore an archived notice back to its previous published/draft state.
        /// </summary>
        [HttpPost("{id:int}/restore")]
        [PermissionAuthorize(PermissionNames.NoticePublish)]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> Restore(int id, CancellationToken cancellationToken)
        {
            var result = await _noticeService.RestoreAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Upload (or replace) the attachment for a notice.
        /// </summary>
        [HttpPost("{id:int}/attachment")]
        [PermissionAuthorize(PermissionNames.NoticeEdit)]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> UploadAttachment(
            int id,
           /* [FromForm]*/ IFormFile attachmentFile,
            CancellationToken cancellationToken)
        {
            var result = await _noticeService.UploadAttachmentAsync(id, attachmentFile, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Remove the attachment from a notice, if any.
        /// </summary>
        [HttpDelete("{id:int}/attachment")]
        [PermissionAuthorize(PermissionNames.NoticeEdit)]
        [ProducesResponseType(typeof(NoticeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NoticeDto>> RemoveAttachment(int id, CancellationToken cancellationToken)
        {
            var result = await _noticeService.RemoveAttachmentAsync(id, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every currently active notice (published, not archived, not expired).
        /// </summary>
        [HttpGet("active")]
        [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(IReadOnlyList<NoticeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NoticeDto>>> GetActive(CancellationToken cancellationToken)
        {
            var result = await _noticeService.GetActiveAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every notice whose publish date is still in the future.
        /// </summary>
        [HttpGet("upcoming")]
        [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(IReadOnlyList<NoticeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NoticeDto>>> GetUpcoming(CancellationToken cancellationToken)
        {
            var result = await _noticeService.GetUpcomingAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every non-archived notice whose expiry date has passed.
        /// </summary>
        [HttpGet("expired")]
       [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(IReadOnlyList<NoticeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NoticeDto>>> GetExpired(CancellationToken cancellationToken)
        {
            var result = await _noticeService.GetExpiredAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get the most recently published notices.
        /// </summary>
        [HttpGet("recent")]
        [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(IReadOnlyList<NoticeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NoticeDto>>> GetRecent(
            [FromQuery] int count = 5,
            CancellationToken cancellationToken = default)
        {
            var result = await _noticeService.GetRecentAsync(count, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get every published, non-archived, high priority notice.
        /// </summary>
        [HttpGet("high-priority")]
        [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(IReadOnlyList<NoticeDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<NoticeDto>>> GetHighPriority(CancellationToken cancellationToken)
        {
            var result = await _noticeService.GetHighPriorityAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get aggregate notice-board statistics for the admin dashboard.
        /// </summary>
        [HttpGet("dashboard")]
        [PermissionAuthorize(PermissionNames.NoticeView)]
        [ProducesResponseType(typeof(NoticeDashboardSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<NoticeDashboardSummaryDto>> GetDashboard(CancellationToken cancellationToken)
        {
            var result = await _noticeService.GetDashboardSummaryAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Delete (soft-delete) a notice.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.NoticeDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _noticeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
