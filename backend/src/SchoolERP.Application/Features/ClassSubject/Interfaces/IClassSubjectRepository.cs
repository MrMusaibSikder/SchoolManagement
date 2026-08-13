using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.ClassSubject.Interfaces;

/// <summary>
/// Repository contract for the <see cref="ClassSubject"/> join entity, which uses a
/// composite key (ClassId, SubjectId) rather than the single-Id base entity shape,
/// so it is not built on the generic repository.
/// </summary>
public interface IClassSubjectRepository
{
    /// <summary>Gets a single association by its composite key.</summary>
    Task<SchoolERP.Domain.Entities.ClassSubject?> GetAsync(int classId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>Gets every association.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ClassSubject>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Checks whether an association already exists.</summary>
    Task<bool> ExistsAsync(int classId, int subjectId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new association.</summary>
    Task<SchoolERP.Domain.Entities.ClassSubject> AddAsync(SchoolERP.Domain.Entities.ClassSubject entity, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing association.</summary>
    void Remove(SchoolERP.Domain.Entities.ClassSubject entity);

    /// <summary>Gets the SubjectIds marked optional for a given class. Used by result calculation to exclude optional subjects from the fail count.</summary>
    Task<IReadOnlyList<int>> GetOptionalSubjectIdsAsync(int classId, CancellationToken cancellationToken = default);

    /// <summary>Sets whether a class-subject association is optional.</summary>
    Task SetOptionalAsync(int classId, int subjectId, bool isOptional, CancellationToken cancellationToken = default);
}
