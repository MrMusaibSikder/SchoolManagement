using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Features.ResultAuditLog.DTOs;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Default <see cref="IResultAuditService"/> implementation. Writes an
/// append-only <see cref="ResultAuditLog"/> row for every Result-management
/// lifecycle action and can replay the history for a given entity.
/// </summary>
public class ResultAuditService : IResultAuditService
{
    private readonly IUnitOfWork _unitOfWork;

    public ResultAuditService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task LogAsync(string entityType, int entityId, ResultAuditAction action, int? performedBy, string? notes = null, CancellationToken cancellationToken = default)
    {
        var entry = new ResultAuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            PerformedBy = performedBy,
            Notes = notes
        };

        await _unitOfWork.ResultAuditLogRepository.AddAsync(entry, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResultAuditLogDto>> GetHistoryAsync(string entityType, int entityId, CancellationToken cancellationToken = default)
    {
        var entries = await _unitOfWork.ResultAuditLogRepository.GetByEntityAsync(entityType, entityId, cancellationToken);

        if (entries.Count == 0)
            return Array.Empty<ResultAuditLogDto>();

        var userIds = entries.Where(x => x.PerformedBy.HasValue).Select(x => x.PerformedBy!.Value).Distinct().ToList();
        var allUsers = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);
        var userNames = allUsers.Where(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id, u => u.Username);

        return entries.Select(x => new ResultAuditLogDto
        {
            Id = x.Id,
            EntityType = x.EntityType,
            EntityId = x.EntityId,
            Action = x.Action,
            PerformedBy = x.PerformedBy,
            PerformedByName = x.PerformedBy.HasValue && userNames.TryGetValue(x.PerformedBy.Value, out var name) ? name : null,
            Notes = x.Notes,
            PerformedAt = x.CreatedAt
        }).ToList();
    }
}
