using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.ExamWeightItem.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="ExamWeightItem"/> entities.
/// Works only with the <see cref="ExamWeightItem"/> entity; never returns DTOs.
/// </summary>
public class ExamWeightItemRepository : GenericRepository<ExamWeightItem>, IExamWeightItemRepository
{
    public ExamWeightItemRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamWeightItem>> GetBySetupAsync(int examWeightSetupId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.Exam)
            .Where(x => !x.IsDeleted && x.ExamWeightSetupId == examWeightSetupId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExamExistsInSetupAsync(int examWeightSetupId, int examId, int? excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.ExamWeightSetupId == examWeightSetupId &&
                x.ExamId == examId &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<decimal> GetTotalWeightAsync(int examWeightSetupId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.ExamWeightSetupId == examWeightSetupId)
            .SumAsync(x => x.WeightPercentage, cancellationToken);
    }
}
