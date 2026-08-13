using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.FeeCategory.Interfaces;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Repositories
{
    public class FeeCategoryRepository : GenericRepository<SchoolERP.Domain.Entities.FeeCategory>, IFeeCategoryRepository
    {
        public FeeCategoryRepository(SchoolERPDbContext context) : base(context) { }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .AnyAsync(x => !x.IsDeleted && x.Name == name && (excludeId == null || x.Id != excludeId), cancellationToken);

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeCategory>> GetActiveOrderedAsync(CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync(cancellationToken);
    }
}
