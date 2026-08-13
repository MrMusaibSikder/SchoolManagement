using SchoolERP.Application.Features.StudentGuardian.DTOs;

namespace SchoolERP.Application.Features.StudentGuardian.Interfaces;

/// <summary>
/// Business/service contract for the StudentGuardian association. Returns DTOs only.
/// </summary>
public interface IStudentGuardianService
{
    /// <summary>Retrieves every StudentGuardian association.</summary>
    Task<IReadOnlyList<StudentGuardianDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single association by its composite key, or null if it does not exist.</summary>
    Task<StudentGuardianDto?> GetAsync(int studentId, int guardianId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new StudentGuardian association.</summary>
    ///Task<StudentGuardianDto> AssignAsync(StudentGuardianDto request, CancellationToken cancellationToken = default);

    /// <summary>Removes an existing StudentGuardian association.</summary>
    Task RemoveAsync(int studentId, int guardianId, CancellationToken cancellationToken = default);
}
