using Microsoft.EntityFrameworkCore;
using SchoolERP.Application.Features.StudentFeeConcession.Interfaces;
using SchoolERP.Infrastructure.Persistence.Context;
using SchoolERP.Infrastructure.Repositories.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Repositories
{
    public class StudentFeeConcessionRepository : GenericRepository<SchoolERP.Domain.Entities.StudentFeeConcession>, IStudentFeeConcessionRepository
    {
        public StudentFeeConcessionRepository(SchoolERPDbContext context) : base(context) { }

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentFeeConcession>> GetByStudentIdAsync(
            int studentId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Include(x => x.FeeType)
                .Include(x => x.AcademicYear)
                .Where(x => !x.IsDeleted && x.StudentId == studentId)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentFeeConcession>> GetPendingApprovalsAsync(
            CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.FeeType)
                .Where(x => !x.IsDeleted && x.RequiresApproval && !x.IsApproved)
                .ToListAsync(cancellationToken);

        public async Task<SchoolERP.Domain.Entities.StudentFeeConcession?> GetActiveForStudentFeeTypeAsync(
            int studentId, int feeTypeId, int academicYearId, CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.IsActive
                                        && (!x.RequiresApproval || x.IsApproved)
                                        && x.StudentId == studentId
                                        && x.FeeTypeId == feeTypeId
                                        && x.AcademicYearId == academicYearId, cancellationToken);
    }
}
