using SchoolERP.Application.Features.Designation.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Designation"/> entities.
/// Works only with the <see cref="Designation"/> entity; never returns DTOs.
/// </summary>
public class DesignationRepository : GenericRepository<Designation>, IDesignationRepository
{
    public DesignationRepository(SchoolERPDbContext context) : base(context)
    {
    }
}
