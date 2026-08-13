using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.FeeCategory.Interfaces
{
    public interface IFeeCategoryRepository : IGenericRepository<SchoolERP.Domain.Entities.FeeCategory>
    {
        Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SchoolERP.Domain.Entities.FeeCategory>> GetActiveOrderedAsync(CancellationToken cancellationToken = default);
    }
}
