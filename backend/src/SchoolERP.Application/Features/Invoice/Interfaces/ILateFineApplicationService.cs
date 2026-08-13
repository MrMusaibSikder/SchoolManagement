using SchoolERP.Application.Features.Invoice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.Interfaces
{
    public interface ILateFineApplicationService
    {
        /// <summary>
        /// Recalculates and applies late fines on every overdue, unpaid invoice's items,
        /// based on the applicable LateFineRule per fee type (falling back to the academic
        /// year's global rule). Idempotent — safe to run daily; fines are recalculated from
        /// scratch each run (not accumulated), so DailyAccrual rules grow naturally over time
        /// while Fixed/Percentage rules stay constant regardless of how many times this runs.
        /// </summary>
        Task<LateFineApplicationResultDto> ApplyLateFinesAsync(
            DateTime? asOfDate = null, CancellationToken cancellationToken = default);
    }
}
