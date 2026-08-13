using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.ResultAuditLog.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="ResultAuditLog"/> entries.
/// Works only with the <see cref="ResultAuditLog"/> entity; never returns DTOs.
/// </summary>
public class ResultAuditLogRepository : GenericRepository<ResultAuditLog>, IResultAuditLogRepository
{
    public ResultAuditLogRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResultAuditLog>> GetByEntityAsync(string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.EntityType == entityType && x.EntityId == entityId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
