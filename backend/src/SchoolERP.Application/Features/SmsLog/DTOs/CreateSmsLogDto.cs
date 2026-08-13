using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>
/// Input model for creating a new SmsLog record. Intended to be called by the
/// system itself (e.g. a future SMS gateway integration writing delivery
/// results), not typically by end users through the UI.
/// </summary>
public class CreateSmsLogDto
{
    public string RecipientNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public SmsStatus Status { get; set; }
    public string? ProviderResponse { get; set; }
    public DateTime? SentAt { get; set; }

    /// <summary>Name of the SMS gateway/provider that delivered (or attempted to deliver) this message.</summary>
    public string? Provider { get; set; }

    public int? StudentId { get; set; }
}
