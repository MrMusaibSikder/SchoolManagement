using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.FinalResult.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="FinalResult"/> entities.
/// Works only with the <see cref="FinalResult"/> entity; never returns DTOs.
/// </summary>
public class FinalResultRepository : GenericRepository<FinalResult>, IFinalResultRepository
{
    public FinalResultRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<FinalResult?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FinalResult?> GetByIdTrackedWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(x => x.Details)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FinalResult?> GetByStudentAndYearAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .FirstOrDefaultAsync(x => x.StudentId == studentId && x.AcademicYearId == academicYearId && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FinalResult>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted)
            .OrderBy(x => x.MeritPosition ?? int.MaxValue)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FinalResult>> GetByAcademicYearAndClassAsync(int academicYearId, int classId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted && x.Student!.ClassId == classId)
            .OrderBy(x => x.ClassPosition ?? int.MaxValue)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FinalResult>> GetByAcademicYearAndSectionAsync(int academicYearId, int sectionId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => x.AcademicYearId == academicYearId && !x.IsDeleted && x.Student!.SectionId == sectionId)
            .OrderBy(x => x.SectionPosition ?? int.MaxValue)
            .ToListAsync(cancellationToken);
    }

    /// <summary>Shared base query eagerly loading Student (+Class/Section), AcademicYear and Details (+Subject).</summary>
    private IQueryable<FinalResult> WithDetailsQuery()
    {
        return DbSet
            .AsNoTracking()
            .Include(x => x.Student!).ThenInclude(s => s.SchoolClass)
            .Include(x => x.Student!).ThenInclude(s => s.Section)
            .Include(x => x.AcademicYear)
            .Include(x => x.Details.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.Subject);
    }
}
