using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.Interfaces
{
    public interface IStudentFeeConcessionService
    {
        Task<IReadOnlyList<StudentFeeConcessionListDto>> GetByStudentIdAsync(int studentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<StudentFeeConcessionListDto>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default);
        Task<StudentFeeConcessionDto> CreateAsync(CreateStudentFeeConcessionDto request, CancellationToken cancellationToken = default);
        Task<StudentFeeConcessionDto> UpdateAsync(int id, UpdateStudentFeeConcessionDto request, CancellationToken cancellationToken = default);
        Task<StudentFeeConcessionDto> ApproveAsync(ApproveConcessionDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
