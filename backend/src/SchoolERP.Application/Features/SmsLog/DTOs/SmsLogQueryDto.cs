using SchoolERP.Application.Common.Models;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>
/// Query parameters for listing SmsLog records: free-text search over
/// recipient/message, status/provider/student/date-range filtering, paging and
/// sorting. <see cref="PagedQueryDto.SearchTerm"/> matches against recipient
/// number and message text.
/// </summary>
public class SmsLogQueryDto : PagedQueryDto
{
    /// <summary>Restricts results to logs with this delivery status.</summary>
    public SmsStatus? Status { get; set; }

    /// <summary>Restricts results to logs sent to this exact recipient number.</summary>
    public string? RecipientNumber { get; set; }

    /// <summary>Restricts results to logs related to this student.</summary>
    public int? StudentId { get; set; }

    /// <summary>Restricts results to logs sent through this provider/gateway.</summary>
    public string? Provider { get; set; }

    /// <summary>Restricts results to logs created on/after this date (inclusive).</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Restricts results to logs created on/before this date (inclusive).</summary>
    public DateTime? ToDate { get; set; }
}
