using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.ResultAuditLog.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.ResultAuditLog"/>
/// entries. Contains database operations only.
/// </summary>
public interface IResultAuditLogRepository : IGenericRepository<SchoolERP.Domain.Entities.ResultAuditLog>
{
    /// <summary>Gets every audit entry for a specific entity, most recent first.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ResultAuditLog>> GetByEntityAsync(
        string entityType,
        int entityId,
        CancellationToken cancellationToken = default);
}
