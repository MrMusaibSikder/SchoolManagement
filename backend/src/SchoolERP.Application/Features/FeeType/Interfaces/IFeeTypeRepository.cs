using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.FeeType.Interfaces;

/// <summary>
/// Repository contract for <see cref="FeeType"/> entities.
/// Extends the generic repository with a FeeType-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IFeeTypeRepository : IGenericRepository<SchoolERP.Domain.Entities.FeeType>
{
    /// <summary>
    /// Loads the Late Fine Rule along with its Fee Category for listing and detail views.
    /// </summary>
    Task<SchoolERP.Domain.Entities.FeeType?> GetWithCategoryAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeType>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
}
