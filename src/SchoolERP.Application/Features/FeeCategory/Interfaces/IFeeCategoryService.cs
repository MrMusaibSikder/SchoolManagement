using SchoolERP.Application.Features.FeeCategory.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeCategory.Interfaces
{
    public interface IFeeCategoryService
    {
        Task<IReadOnlyList<FeeCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<FeeCategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<FeeCategoryDto> CreateAsync(CreateFeeCategoryDto request, CancellationToken cancellationToken = default);
        Task<FeeCategoryDto> UpdateAsync(int id, UpdateFeeCategoryDto request, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
