using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.ResultAuditLog.DTOs;

/// <summary>Read model for a single audit trail entry.</summary>
public class ResultAuditLogDto
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public ResultAuditAction Action { get; set; }
    public int? PerformedBy { get; set; }
    public string? PerformedByName { get; set; }
    public string? Notes { get; set; }
    public DateTime PerformedAt { get; set; }
}
