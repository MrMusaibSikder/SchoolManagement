using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.ExamType.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="ExamType"/> entities.
/// Works only with the <see cref="ExamType"/> entity; never returns DTOs.
/// </summary>
public class ExamTypeRepository : GenericRepository<ExamType>, IExamTypeRepository
{
    public ExamTypeRepository(SchoolERPDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .AnyAsync(x =>
                !x.IsDeleted &&
                x.Name.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId.Value),
                cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> IsInUseAsync(int examTypeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Exam>()
            .AsNoTracking()
            .AnyAsync(x => !x.IsDeleted && x.ExamTypeId == examTypeId, cancellationToken);
    }
}
