using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.FeeStructure.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="FeeStructure"/> entities.
/// Works only with the <see cref="FeeStructure"/> entity; never returns DTOs.
/// </summary>
public class FeeStructureRepository : GenericRepository<FeeStructure>, IFeeStructureRepository
{
    public FeeStructureRepository(SchoolERPDbContext context) : base(context)
    {

    }
    public async Task<SchoolERP.Domain.Entities.FeeStructure?> GetApplicableStructureAsync(
         int schoolClassId, int? sectionId, int academicYearId, CancellationToken cancellationToken = default)
    {
        // Section-specific structure আগে চেক, না পেলে class-wide (SectionId == null) fallback
        var sectionSpecific = await DbSet.AsNoTracking()
            .Include(x => x.FeeStructureItems).ThenInclude(i => i.FeeType)
            .Where(x => !x.IsDeleted && x.IsActive
                     && x.SchoolClassId == schoolClassId
                     && x.AcademicYearId == academicYearId
                     && x.SectionId == sectionId)
            .FirstOrDefaultAsync(cancellationToken);

        if (sectionSpecific != null || sectionId == null)
            return sectionSpecific;

        return await DbSet.AsNoTracking()
            .Include(x => x.FeeStructureItems).ThenInclude(i => i.FeeType)
            .Where(x => !x.IsDeleted && x.IsActive
                     && x.SchoolClassId == schoolClassId
                     && x.AcademicYearId == academicYearId
                     && x.SectionId == null)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SchoolERP.Domain.Entities.FeeStructure?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Include(x => x.AcademicYear)
            .Include(x => x.SchoolClass)
            .Include(x => x.Section)
            .Include(x => x.FeeStructureItems).ThenInclude(i => i.FeeType)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<SchoolERP.Domain.Entities.FeeStructure?> GetWithItemsTrackedAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet
            .Include(x => x.FeeStructureItems)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeStructure>> GetListAsync(
        int? academicYearId, int? schoolClassId, bool? isActive, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking()
            .Include(x => x.AcademicYear)
            .Include(x => x.SchoolClass)
            .Include(x => x.Section)
            .Include(x => x.FeeStructureItems)
            .Where(x => !x.IsDeleted);

        if (academicYearId.HasValue) query = query.Where(x => x.AcademicYearId == academicYearId);
        if (schoolClassId.HasValue) query = query.Where(x => x.SchoolClassId == schoolClassId);
        if (isActive.HasValue) query = query.Where(x => x.IsActive == isActive);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeStructure>> GetTemplatesAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsTemplate)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsForClassSectionYearAsync(
        int schoolClassId, int? sectionId, int academicYearId, int? excludeId = null, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .AnyAsync(x => !x.IsDeleted
                        && x.SchoolClassId == schoolClassId
                        && x.SectionId == sectionId
                        && x.AcademicYearId == academicYearId
                        && (excludeId == null || x.Id != excludeId), cancellationToken);
}


