using SchoolERP.Application.Features.School.DTOs;

namespace SchoolERP.Application.Features.School.Interfaces;

/// <summary>
/// Business/service contract for School records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface ISchoolService
{
    /// <summary>Retrieves every School record.</summary>
    Task<IReadOnlyList<SchoolDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single School record by id, or null if it does not exist.</summary>
    Task<SchoolDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new School record.</summary>
    Task<SchoolDto> CreateAsync(CreateSchoolDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing School record.</summary>
    Task<SchoolDto> UpdateAsync(int id, UpdateSchoolDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing School record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
