using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Guardian.Interfaces;

/// <summary>
/// Repository contract for <see cref="Guardian"/> entities.
/// Extends the generic repository with a Guardian-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IGuardianRepository : IGenericRepository<SchoolERP.Domain.Entities.Guardian>
{
    Task<IReadOnlyList<SchoolERP.Domain.Entities.Guardian>> SearchAsync(
       string keyword,
       CancellationToken cancellationToken = default);

    Task<bool> PhoneExistsAsync(
        string phoneNumber,
        CancellationToken cancellationToken = default);
}
