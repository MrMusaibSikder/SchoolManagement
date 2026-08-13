using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>Input model for updating an existing SmsLog record.</summary>
public class UpdateSmsLogDto
{
    public int Id { get; set; }
    public string RecipientNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public SmsStatus Status { get; set; }
    public string? ProviderResponse { get; set; }
    public DateTime? SentAt { get; set; }
    public int? StudentId { get; set; }
}
