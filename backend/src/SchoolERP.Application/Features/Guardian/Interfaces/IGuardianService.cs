using SchoolERP.Application.Features.Guardian.DTOs;

namespace SchoolERP.Application.Features.Guardian.Interfaces;

/// <summary>
/// Business/service contract for Guardian records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IGuardianService
{
    /// <summary>Retrieves every Guardian record.</summary>
    Task<IReadOnlyList<GuardianDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Guardian record by id, or null if it does not exist.</summary>
    Task<GuardianDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Guardian record.</summary>
    Task<GuardianDto> CreateAsync(CreateGuardianDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Guardian record.</summary>
    Task<GuardianDto> UpdateAsync(int id, UpdateGuardianDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Guardian record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    /// <summary>
    /// for search Add By Musaib Sikder
    /// </summary>
    /// <returns></returns>
    Task<IReadOnlyList<GuardianDto>> SearchAsync(
    string keyword,
    CancellationToken cancellationToken = default);
}
