using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.Student.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="Student"/> entities.
/// Works only with the <see cref="Student"/> entity; never returns DTOs.
/// </summary>
public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(SchoolERPDbContext context) : base(context)
    {
    }
    public async Task<Student?> GetByIdWithGuardiansAsync(
       int id,
       CancellationToken cancellationToken = default)
    {
        return await Context.Students
            .Include(x => x.StudentGuardians)
            .ThenInclude(x => x.Guardian)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }


    public async Task<Student?> GetByIdWithGuardiansTrackedAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await Context.Students
            .Include(x => x.StudentGuardians)
            .ThenInclude(x => x.Guardian)
            .FirstOrDefaultAsync(
                x => x.Id == id && !x.IsDeleted,
                cancellationToken);
    }
    public async Task<IReadOnlyList<Student>> GetAllWithGuardiansAsync(
    CancellationToken cancellationToken = default)
    {
        return await Context.Students
            .Include(x => x.StudentGuardians)
                .ThenInclude(x => x.Guardian)
            .Where(x => !x.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
    public async Task<IReadOnlyList<Student>> GetByClassAndSectionAsync(
    int classId,
    int sectionId,
    CancellationToken cancellationToken = default)
    {
        return await DbSet
    .AsNoTracking()
    .Where(x =>
        !x.IsDeleted &&
        x.ClassId == classId &&
        x.SectionId == sectionId)
    .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SchoolERP.Domain.Entities.Student>> GetActiveByClassSectionAsync(
    int schoolClassId, int? sectionId, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking()
            .Where(x => !x.IsDeleted && x.ClassId == schoolClassId);

        if (sectionId.HasValue)
            query = query.Where(x => x.SectionId == sectionId);

        return await query.ToListAsync(cancellationToken);
    }
}
