using SchoolERP.Application.Features.FeeType.DTOs;

namespace SchoolERP.Application.Features.FeeType.Interfaces;

/// <summary>
/// Business/service contract for FeeType records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IFeeTypeService
{
    Task<IReadOnlyList<FeeTypeListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FeeTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FeeTypeDto> CreateAsync(CreateFeeTypeDto request, CancellationToken cancellationToken = default);
    Task<FeeTypeDto> UpdateAsync(int id, UpdateFeeTypeDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
