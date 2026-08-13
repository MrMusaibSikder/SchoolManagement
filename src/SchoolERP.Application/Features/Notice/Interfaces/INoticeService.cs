using Microsoft.AspNetCore.Http;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.Notice.DTOs;

namespace SchoolERP.Application.Features.Notice.Interfaces;

/// <summary>
/// Business/service contract for Notice records. Services return DTOs only
/// and encapsulate all business rules for this feature: publish/unpublish/
/// archive/restore workflow, attachment handling, duplicate/date validation
/// and dashboard aggregation.
/// </summary>
public interface INoticeService
{
    /// <summary>Retrieves every Notice record.</summary>
    Task<IReadOnlyList<NoticeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a search-filtered, paged, sorted list of Notice records.</summary>
    Task<PagedResult<NoticeDto>> GetPagedAsync(NoticeQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Notice record by id, or null if it does not exist.</summary>
    Task<NoticeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Notice record as a draft (not published). Titles must be unique.</summary>
    Task<NoticeDto> CreateAsync(CreateNoticeDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Notice record. Titles must be unique. Does not change publish/archive state.</summary>
    Task<NoticeDto> UpdateAsync(int id, UpdateNoticeDto request, CancellationToken cancellationToken = default);

    /// <summary>Publishes a notice, making it visible to its target audience.</summary>
    Task<NoticeDto> PublishAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Reverts a published notice back to draft state.</summary>
    Task<NoticeDto> UnpublishAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Archives a notice, retiring it from active circulation.</summary>
    Task<NoticeDto> ArchiveAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Restores an archived notice back to its previous (published or draft) state.</summary>
    Task<NoticeDto> RestoreAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Uploads (or replaces) the attachment for an existing notice.</summary>
    Task<NoticeDto> UploadAttachmentAsync(int id, IFormFile attachmentFile, CancellationToken cancellationToken = default);

    /// <summary>Removes the attachment from an existing notice, if any.</summary>
    Task<NoticeDto> RemoveAttachmentAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets every currently active notice (published, not archived, not expired).</summary>
    Task<IReadOnlyList<NoticeDto>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets every notice whose publish date is still in the future.</summary>
    Task<IReadOnlyList<NoticeDto>> GetUpcomingAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets every non-archived notice whose expiry date has passed.</summary>
    Task<IReadOnlyList<NoticeDto>> GetExpiredAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the most recently published notices.</summary>
    Task<IReadOnlyList<NoticeDto>> GetRecentAsync(int count, CancellationToken cancellationToken = default);

    /// <summary>Gets every published, non-archived, high priority notice.</summary>
    Task<IReadOnlyList<NoticeDto>> GetHighPriorityAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets aggregate notice-board statistics for the admin dashboard.</summary>
    Task<NoticeDashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Notice record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
