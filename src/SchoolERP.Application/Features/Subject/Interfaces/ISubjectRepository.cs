using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Subject.Interfaces;

/// <summary>
/// Repository contract for <see cref="Subject"/> entities.
/// Extends the generic repository with a Subject-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface ISubjectRepository : IGenericRepository<SchoolERP.Domain.Entities.Subject>
{

}
