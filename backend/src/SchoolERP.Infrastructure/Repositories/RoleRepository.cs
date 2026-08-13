using SchoolERP.Application.Features.Role.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Role"/> entities.
/// Works only with the <see cref="Role"/> entity; never returns DTOs.
/// </summary>
public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
