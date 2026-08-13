using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Teacher.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Teacher"/> entities.
/// Works only with the <see cref="Teacher"/> entity; never returns DTOs.
/// </summary>
public class TeacherRepository : GenericRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(SchoolERPDbContext context) : base(context)
    {
    }
    public async Task<bool> ExistsByEmployeeIdAsync(
      int employeeId,
      CancellationToken cancellationToken = default)
    {
        return await DbSet
    .AsNoTracking()
    .AnyAsync(x => x.EmployeeId == employeeId, cancellationToken);
    }
}
