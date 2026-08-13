using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Features.Permission.Interfaces;

/// <summary>
/// Repository contract for <see cref="Permission"/> entities.
/// Extends the generic repository with a Permission-specific data access members
/// as they become necessary. Contains database operations only.
/// </summary>
public interface IPermissionRepository : IGenericRepository<SchoolERP.Domain.Entities.Permission>
{
}
