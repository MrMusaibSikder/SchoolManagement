using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.SubjectTeacher.Interfaces;

/// <summary>
/// Repository contract for the <see cref="SubjectTeacher"/> join entity, which uses a
/// composite key (SubjectId, TeacherId) rather than the single-Id base entity shape,
/// so it is not built on the generic repository.
/// </summary>
public interface ISubjectTeacherRepository
{
    /// <summary>Gets a single association by its composite key.</summary>
    Task<SchoolERP.Domain.Entities.SubjectTeacher?> GetAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default);

    /// <summary>Gets every association.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.SubjectTeacher>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Checks whether an association already exists.</summary>
    Task<bool> ExistsAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new association.</summary>
    Task<SchoolERP.Domain.Entities.SubjectTeacher> AddAsync(SchoolERP.Domain.Entities.SubjectTeacher entity, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing association.</summary>
    void Remove(SchoolERP.Domain.Entities.SubjectTeacher entity);
}
