using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>
/// Immutable audit trail entry recording a single lifecycle action against a
/// Result-management record (a mark entry, an <see cref="ExamResult"/>, or a
/// <see cref="FinalResult"/>). Never updated or soft-deleted after creation.
/// </summary>
public class ResultAuditLog : BaseEntity
{
    /// <summary>The entity type this entry concerns: "Result", "ExamResult", or "FinalResult".</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>Id of the affected entity (Result, ExamResult, or FinalResult row).</summary>
    public int EntityId { get; set; }

    /// <summary>The action performed.</summary>
    public ResultAuditAction Action { get; set; }

    /// <summary>Id of the User who performed the action, if known.</summary>
    public int? PerformedBy { get; set; }

    /// <summary>Optional free-text context (e.g. reason for a rollback).</summary>
    public string? Notes { get; set; }
}
