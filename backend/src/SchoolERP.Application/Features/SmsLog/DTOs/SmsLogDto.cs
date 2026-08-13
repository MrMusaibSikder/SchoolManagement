using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>Read model returned to clients for a SmsLog record.</summary>
public class SmsLogDto
{
    public int Id { get; set; }
    public string RecipientNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public SmsStatus Status { get; set; }
    public string? ProviderResponse { get; set; }
    public DateTime? SentAt { get; set; }

    /// <summary>Name of the SMS gateway/provider that delivered (or attempted to deliver) this message.</summary>
    public string? Provider { get; set; }

    public int? StudentId { get; set; }

    /// <summary>Full name of the related student, when <see cref="StudentId"/> is set.</summary>
    public string? StudentName { get; set; }

    /// <summary>UTC timestamp when the log record was created.</summary>
    public DateTime CreatedAt { get; set; }
}
