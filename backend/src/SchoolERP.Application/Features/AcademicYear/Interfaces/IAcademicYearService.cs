using SchoolERP.Application.Features.AcademicYear.DTOs;

namespace SchoolERP.Application.Features.AcademicYear.Interfaces;

/// <summary>
/// Business/service contract for AcademicYear records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IAcademicYearService
{
    /// <summary>Retrieves every AcademicYear record.</summary>
    Task<IReadOnlyList<AcademicYearDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single AcademicYear record by id, or null if it does not exist.</summary>
    Task<AcademicYearDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new AcademicYear record.</summary>
    Task<AcademicYearDto> CreateAsync(CreateAcademicYearDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing AcademicYear record.</summary>
    Task<AcademicYearDto> UpdateAsync(int id, UpdateAcademicYearDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing AcademicYear record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
