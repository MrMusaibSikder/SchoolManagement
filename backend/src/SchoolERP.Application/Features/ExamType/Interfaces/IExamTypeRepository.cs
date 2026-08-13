using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.ExamType.Interfaces;

/// <summary>
/// Repository contract for <see cref="ExamType"/> entities.
/// Extends the generic repository with an ExamType-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IExamTypeRepository : IGenericRepository<SchoolERP.Domain.Entities.ExamType>
{
    /// <summary>Checks whether another (non-deleted) exam type already has the given name (case-insensitive).</summary>
    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default);

    /// <summary>Checks whether any (non-deleted) exam currently references this exam type.</summary>
    Task<bool> IsInUseAsync(int examTypeId, CancellationToken cancellationToken = default);
}
