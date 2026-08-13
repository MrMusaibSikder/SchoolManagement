using SchoolERP.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.LateFineRule.Interfaces
{
    public interface ILateFineRuleRepository : IGenericRepository<SchoolERP.Domain.Entities.LateFineRule>
    {
        /// <summary>
        /// Retrieves the rule for the specified Fee Type. If no specific rule exists,
        /// falls back to the global rule (FeeTypeId == null). This is the primary query
        /// used for late fine calculation.
        /// </summary>
        Task<SchoolERP.Domain.Entities.LateFineRule?> GetApplicableRuleAsync(
            int academicYearId, int feeTypeId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SchoolERP.Domain.Entities.LateFineRule>> GetByAcademicYearAsync(
            int academicYearId, CancellationToken cancellationToken = default);
    }
}
