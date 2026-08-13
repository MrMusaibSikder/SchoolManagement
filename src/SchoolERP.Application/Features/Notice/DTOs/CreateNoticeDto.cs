using Microsoft.AspNetCore.Http;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Notice.DTOs;

/// <summary>Input model for creating a new Notice record.</summary>
public class CreateNoticeDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public NoticePriority Priority { get; set; } = NoticePriority.Medium;
    public NoticeAudience Audience { get; set; } = NoticeAudience.Everyone;

    /// <summary>Whether an SMS should be sent for this notice (architecture only; sending is a future integration).</summary>
    public bool SendSms { get; set; }

    /// <summary>Whether an Email should be sent for this notice (architecture only; sending is a future integration).</summary>
    public bool SendEmail { get; set; }

    /// <summary>Optional attachment file, uploaded alongside the notice.</summary>
    public IFormFile? AttachmentFile { get; set; }
}
