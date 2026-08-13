using SchoolERP.Application.Features.Permission.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Permission"/> entities.
/// Works only with the <see cref="Permission"/> entity; never returns DTOs.
/// </summary>
public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
