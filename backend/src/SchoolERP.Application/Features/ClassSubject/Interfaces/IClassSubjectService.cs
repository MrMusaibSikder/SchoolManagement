using SchoolERP.Application.Features.ClassSubject.DTOs;

namespace SchoolERP.Application.Features.ClassSubject.Interfaces;

/// <summary>
/// Business/service contract for the ClassSubject association. Returns DTOs only.
/// </summary>
public interface IClassSubjectService
{
    /// <summary>Retrieves every ClassSubject association.</summary>
    Task<IReadOnlyList<ClassSubjectDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single association by its composite key, or null if it does not exist.</summary>
    Task<ClassSubjectDto?> GetAsync(int classId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new ClassSubject association.</summary>
    Task<ClassSubjectDto> AssignAsync(ClassSubjectDto request, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing ClassSubject association.</summary>
    Task RemoveAsync(int classId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>Marks an existing ClassSubject association as optional or mandatory.</summary>
    Task<ClassSubjectDto> SetOptionalAsync(int classId, int subjectId, bool isOptional, CancellationToken cancellationToken = default);
}
