using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Role.Interfaces;

/// <summary>
/// Repository contract for <see cref="Role"/> entities.
/// Extends the generic repository with a Role-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IRoleRepository : IGenericRepository<SchoolERP.Domain.Entities.Role>
{
}
