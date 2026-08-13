using SchoolERP.Application.Features.Student.DTOs;

namespace SchoolERP.Application.Features.Student.Interfaces;

/// <summary>
/// Business/service contract for Student records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IStudentService
{
    /// <summary>Retrieves every Student record.</summary>
    Task<IReadOnlyList<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Student record by id, or null if it does not exist.</summary>
    Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Student record.</summary>
    Task<StudentDto> CreateAsync(CreateStudentDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Student record.</summary>
    Task<StudentDto> UpdateAsync(int id, UpdateStudentDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Student record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
