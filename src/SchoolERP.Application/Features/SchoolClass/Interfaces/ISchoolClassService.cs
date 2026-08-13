using SchoolERP.Application.Features.SchoolClass.DTOs;

namespace SchoolERP.Application.Features.SchoolClass.Interfaces;

/// <summary>
/// Business/service contract for SchoolClass records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface ISchoolClassService
{
    /// <summary>Retrieves every SchoolClass record.</summary>
    Task<IReadOnlyList<SchoolClassDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single SchoolClass record by id, or null if it does not exist.</summary>
    Task<SchoolClassDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new SchoolClass record.</summary>
    Task<SchoolClassDto> CreateAsync(CreateSchoolClassDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing SchoolClass record.</summary>
    Task<SchoolClassDto> UpdateAsync(int id, UpdateSchoolClassDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing SchoolClass record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
