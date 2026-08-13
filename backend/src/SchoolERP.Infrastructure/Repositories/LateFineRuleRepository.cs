using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.LateFineRule.Interfaces;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Repositories
{
    public class LateFineRuleRepository : GenericRepository<SchoolERP.Domain.Entities.LateFineRule>, ILateFineRuleRepository
    {
        public LateFineRuleRepository(SchoolERPDbContext context) : base(context) { }

        public async Task<SchoolERP.Domain.Entities.LateFineRule?> GetApplicableRuleAsync(
            int academicYearId, int feeTypeId, CancellationToken cancellationToken = default)
        {
            var specific = await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsActive
                                        && x.AcademicYearId == academicYearId
                                        && x.FeeTypeId == feeTypeId, cancellationToken);

            if (specific != null) return specific;

            return await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.IsActive
                                        && x.AcademicYearId == academicYearId
                                        && x.FeeTypeId == null, cancellationToken);
        }

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.LateFineRule>> GetByAcademicYearAsync(
            int academicYearId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Include(x => x.FeeType)
                .Where(x => x.AcademicYearId == academicYearId)
                .ToListAsync(cancellationToken);
    }
}
