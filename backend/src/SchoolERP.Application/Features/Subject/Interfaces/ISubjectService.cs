using SchoolERP.Application.Features.Subject.DTOs;

namespace SchoolERP.Application.Features.Subject.Interfaces;

/// <summary>
/// Business/service contract for Subject records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface ISubjectService
{
    /// <summary>Retrieves every Subject record.</summary>
    Task<IReadOnlyList<SubjectDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Subject record by id, or null if it does not exist.</summary>
    Task<SubjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Subject record.</summary>
    Task<SubjectDto> CreateAsync(CreateSubjectDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Subject record.</summary>
    Task<SubjectDto> UpdateAsync(int id, UpdateSubjectDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Subject record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
