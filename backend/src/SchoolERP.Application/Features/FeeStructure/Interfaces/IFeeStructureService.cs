using SchoolERP.Application.Features.FeeStructure.DTOs;

namespace SchoolERP.Application.Features.FeeStructure.Interfaces;

/// <summary>
/// Business/service contract for FeeStructure records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>

public interface IFeeStructureService
{
    Task<IReadOnlyList<FeeStructureListDto>> GetListAsync(
        int? academicYearId, int? schoolClassId, bool? isActive, CancellationToken cancellationToken = default);
    Task<FeeStructureDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FeeStructureDto> CreateAsync(CreateFeeStructureDto request, CancellationToken cancellationToken = default);
    Task<FeeStructureDto> UpdateAsync(int id, UpdateFeeStructureDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
