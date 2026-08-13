using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Notice.DTOs;

/// <summary>
/// Query parameters for listing Notice records: free-text search over
/// title/description, audience/priority/publish-state filtering, a publish
/// date range, paging and sorting.
/// </summary>
public class NoticeQueryDto : PagedQueryDto
{
    /// <summary>Restricts results to notices targeting this audience.</summary>
    public NoticeAudience? Audience { get; set; }

    /// <summary>Restricts results to notices with this priority.</summary>
    public NoticePriority? Priority { get; set; }

    /// <summary>Restricts results to published (true) or draft (false) notices.</summary>
    public bool? IsPublished { get; set; }

    /// <summary>Restricts results to archived (true) or non-archived (false) notices.</summary>
    public bool? IsArchived { get; set; }

    /// <summary>Restricts results to notices with a publish date on/after this date.</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Restricts results to notices with a publish date on/before this date.</summary>
    public DateTime? ToDate { get; set; }
}
