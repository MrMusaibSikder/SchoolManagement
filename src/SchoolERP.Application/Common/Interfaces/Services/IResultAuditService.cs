using SchoolERP.Application.Features.ResultAuditLog.DTOs;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Common.Interfaces.Services;

/// <summary>
/// Records and retrieves the audit trail for Result-management actions
/// (mark updates, calculation, verification, publish/unpublish, lock/unlock,
/// archive, rollback). Consumed by <c>ResultService</c>,
/// <c>ExamResultService</c> and <c>FinalResultService</c> so every state
/// change is traceable to a user and a timestamp.
/// </summary>
public interface IResultAuditService
{
    /// <summary>Records a single audit entry. Never throws for logging failures beyond normal persistence errors — callers should not have their primary operation blocked by audit-log issues in a well-configured database.</summary>
    Task LogAsync(string entityType, int entityId, ResultAuditAction action, int? performedBy, string? notes = null, CancellationToken cancellationToken = default);

    /// <summary>Gets the full audit history for a specific entity, most recent first.</summary>
    Task<IReadOnlyList<ResultAuditLogDto>> GetHistoryAsync(string entityType, int entityId, CancellationToken cancellationToken = default);
}
