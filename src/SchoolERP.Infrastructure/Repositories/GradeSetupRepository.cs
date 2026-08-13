using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.GradeSetup.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="GradeSetup"/> entities.
/// Works only with the <see cref="GradeSetup"/> entity; never returns DTOs.
/// </summary>
public class GradeSetupRepository : GenericRepository<GradeSetup>, IGradeSetupRepository
{
    public GradeSetupRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GradeSetup>> GetActiveByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GradeSetup>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.AcademicYear)
            .Where(x => !x.IsDeleted && x.AcademicYearId == academicYearId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> NameExistsAsync(int academicYearId, string gradeName, int? excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.AcademicYearId == academicYearId &&
                x.GradeName.ToLower() == gradeName.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }
}
