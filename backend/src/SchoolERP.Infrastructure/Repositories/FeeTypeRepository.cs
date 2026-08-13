using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.FeeType.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core repository implementation for <see cref="FeeType"/> entities.
/// Works only with the <see cref="FeeType"/> entity; never returns DTOs.
/// </summary>
public class FeeTypeRepository : GenericRepository<FeeType>, IFeeTypeRepository
{
    public FeeTypeRepository(SchoolERPDbContext context) : base(context)
    { }

         public async Task<SchoolERP.Domain.Entities.FeeType?> GetWithCategoryAsync(int id, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Include(x => x.FeeCategory)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

    public async Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeType>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .Include(x => x.FeeCategory)
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .AnyAsync(x => !x.IsDeleted && x.Code == code && (excludeId == null || x.Id != excludeId), cancellationToken);

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking()
            .AnyAsync(x => !x.IsDeleted && x.Name == name && (excludeId == null || x.Id != excludeId), cancellationToken);
}

