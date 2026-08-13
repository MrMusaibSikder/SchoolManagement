using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a published notice/announcement.</summary>
public class Notice : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }

    /// <summary>Date the notice stops being considered active. Null means it never expires.</summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>Urgency level of the notice.</summary>
    public NoticePriority Priority { get; set; } = NoticePriority.Medium;

    /// <summary>Who the notice is intended for.</summary>
    public NoticeAudience Audience { get; set; } = NoticeAudience.Everyone;

    /// <summary>Whether the notice has been published (visible to its audience).</summary>
    public bool IsPublished { get; set; }

    /// <summary>Whether the notice has been archived (retired from active circulation).</summary>
    public bool IsArchived { get; set; }

    /// <summary>Whether an SMS should be sent for this notice (architecture only; actual sending is a future integration).</summary>
    public bool SendSms { get; set; }

    /// <summary>Whether an Email should be sent for this notice (architecture only; actual sending is a future integration).</summary>
    public bool SendEmail { get; set; }

    /// <summary>Relative storage path of an optional attachment.</summary>
    public string? AttachmentPath { get; set; }
}
