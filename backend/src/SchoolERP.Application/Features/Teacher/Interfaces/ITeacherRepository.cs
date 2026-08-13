using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Teacher.Interfaces;

/// <summary>
/// Repository contract for <see cref="Teacher"/> entities.
/// Extends the generic repository with a Teacher-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface ITeacherRepository : IGenericRepository<SchoolERP.Domain.Entities.Teacher>
{
    Task<bool> ExistsByEmployeeIdAsync(
       int employeeId,
       CancellationToken cancellationToken = default);
}
