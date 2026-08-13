using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Notice.DTOs;

/// <summary>Read model returned to clients for a Notice record.</summary>
public class NoticeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public NoticePriority Priority { get; set; }
    public NoticeAudience Audience { get; set; }
    public bool IsPublished { get; set; }
    public bool IsArchived { get; set; }
    public bool SendSms { get; set; }
    public bool SendEmail { get; set; }
    public string? AttachmentPath { get; set; }

    /// <summary>Computed: true when published, not archived and not past its expiry date.</summary>
    public bool IsActive { get; set; }

    /// <summary>Computed: true when the publish date is in the future.</summary>
    public bool IsUpcoming { get; set; }

    /// <summary>Computed: true when the expiry date has passed.</summary>
    public bool IsExpired { get; set; }

    /// <summary>UTC timestamp when the notice was created.</summary>
    public DateTime CreatedAt { get; set; }
}
