using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a record of an outgoing SMS and its delivery status.</summary>
public class SmsLog : BaseEntity
{
    public string RecipientNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public SmsStatus Status { get; set; }
    public string? ProviderResponse { get; set; }
    public DateTime? SentAt { get; set; }

    /// <summary>Name of the SMS gateway/provider that delivered (or attempted to deliver) this message.</summary>
    public string? Provider { get; set; }

    public int? StudentId { get; set; }
    public Student? Student { get; set; }
}
