using SchoolERP.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.Interfaces
{
    public interface IStudentFeeConcessionRepository : IGenericRepository<SchoolERP.Domain.Entities.StudentFeeConcession>
    {
        Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentFeeConcession>> GetByStudentIdAsync(
            int studentId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<SchoolERP.Domain.Entities.StudentFeeConcession>> GetPendingApprovalsAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the active and approved concession for the student.
        /// Used during invoice generation.
        /// </summary>
        Task<SchoolERP.Domain.Entities.StudentFeeConcession?> GetActiveForStudentFeeTypeAsync(
            int studentId, int feeTypeId, int academicYearId, CancellationToken cancellationToken = default);
    }
}
