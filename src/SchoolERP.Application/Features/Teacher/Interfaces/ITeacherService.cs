using SchoolERP.Application.Features.Teacher.DTOs;

namespace SchoolERP.Application.Features.Teacher.Interfaces;

/// <summary>
/// Business/service contract for Teacher records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface ITeacherService
{
    /// <summary>Retrieves every Teacher record.</summary>
    Task<IReadOnlyList<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Teacher record by id, or null if it does not exist.</summary>
    Task<TeacherDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Teacher record.</summary>
    Task<TeacherDto> CreateAsync(CreateTeacherDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Teacher record.</summary>
    Task<TeacherDto> UpdateAsync(int id, UpdateTeacherDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Teacher record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
