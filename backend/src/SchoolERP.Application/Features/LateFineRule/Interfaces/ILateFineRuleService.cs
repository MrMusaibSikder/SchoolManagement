using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.LateFineRule.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.LateFineRule.Interfaces
{
    public interface ILateFineRuleService
    {
        Task<IReadOnlyList<LateFineRuleDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);
        Task<LateFineRuleDto> CreateAsync(CreateLateFineRuleDto request, CancellationToken cancellationToken = default);
        Task<LateFineRuleDto> UpdateAsync(int id, UpdateLateFineRuleDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
