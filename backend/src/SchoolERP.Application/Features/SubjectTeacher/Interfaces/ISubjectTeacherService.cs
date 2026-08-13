using SchoolERP.Application.Features.SubjectTeacher.DTOs;

namespace SchoolERP.Application.Features.SubjectTeacher.Interfaces;

/// <summary>
/// Business/service contract for the SubjectTeacher association. Returns DTOs only.
/// </summary>
public interface ISubjectTeacherService
{
    /// <summary>Retrieves every SubjectTeacher association.</summary>
    Task<IReadOnlyList<SubjectTeacherDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single association by its composite key, or null if it does not exist.</summary>
    Task<SubjectTeacherDto?> GetAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new SubjectTeacher association.</summary>
    Task<SubjectTeacherDto> AssignAsync(SubjectTeacherDto request, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing SubjectTeacher association.</summary>
    Task RemoveAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default);
}
