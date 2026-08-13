using SchoolERP.Application.Features.GradeSetup.DTOs;

namespace SchoolERP.Application.Features.GradeSetup.Interfaces;

/// <summary>
/// Business/service contract for managing configurable grade bands. Services
/// return DTOs only. Validates that a year's bands don't overlap and enforces
/// unique grade names per academic year.
/// </summary>
public interface IGradeSetupService
{
    /// <summary>Retrieves every grade band.</summary>
    Task<IReadOnlyList<GradeSetupDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single grade band by id, or null if it does not exist.</summary>
    Task<GradeSetupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every grade band configured for an academic year, ordered by DisplayOrder.</summary>
    Task<IReadOnlyList<GradeSetupDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new grade band. The percentage range must not overlap any other active band in the same academic year.</summary>
    Task<GradeSetupDto> CreateAsync(CreateGradeSetupDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing grade band. The percentage range must not overlap any other active band in the same academic year.</summary>
    Task<GradeSetupDto> UpdateAsync(int id, UpdateGradeSetupDto request, CancellationToken cancellationToken = default);

    /// <summary>Activates a grade band.</summary>
    Task<GradeSetupDto> ActivateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Deactivates a grade band without deleting it.</summary>
    Task<GradeSetupDto> DeactivateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes a grade band.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
