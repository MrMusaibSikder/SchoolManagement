using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.ExamWeightSetup.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="ExamWeightSetup"/> entities.
/// Works only with the <see cref="ExamWeightSetup"/> entity; never returns DTOs.
/// </summary>
public class ExamWeightSetupRepository : GenericRepository<ExamWeightSetup>, IExamWeightSetupRepository
{
    public ExamWeightSetupRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetup?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamWeightSetup>> GetAllWithItemsAsync(CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamWeightSetup>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamWeightSetup?> GetActiveByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await WithDetailsQuery()
            .FirstOrDefaultAsync(x => !x.IsDeleted && x.AcademicYearId == academicYearId && x.IsActive, cancellationToken);
    }

    /// <summary>Shared base query eagerly loading Items (+Exam) and AcademicYear.</summary>
    private IQueryable<ExamWeightSetup> WithDetailsQuery()
    {
        return DbSet
            .AsNoTracking()
            .Include(x => x.AcademicYear)
            .Include(x => x.Items.Where(i => !i.IsDeleted))
                .ThenInclude(i => i.Exam);
    }
}
