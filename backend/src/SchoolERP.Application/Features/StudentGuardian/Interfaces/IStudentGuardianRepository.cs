using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.StudentGuardian.Interfaces;

/// <summary>
/// Repository contract for the <see cref="StudentGuardian"/> join entity, which uses a
/// composite key (StudentId, GuardianId) rather than the single-Id base entity shape,
/// so it is not built on the generic repository.
/// </summary>
public interface IStudentGuardianRepository
{
    /// <summary>Gets a single association by its composite key.</summary>
    Task<SchoolERP.Domain.Entities.StudentGuardian?> GetAsync(int studentId, int guardianId, CancellationToken cancellationToken = default);

    /// <summary>Gets every association.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentGuardian>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Checks whether an association already exists.</summary>
    Task<bool> ExistsAsync(int studentId, int guardianId, CancellationToken cancellationToken = default);

    /// <summary>Adds a new association.</summary>
    Task<SchoolERP.Domain.Entities.StudentGuardian> AddAsync(SchoolERP.Domain.Entities.StudentGuardian entity, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing association.</summary>
    void Remove(SchoolERP.Domain.Entities.StudentGuardian entity);
}
