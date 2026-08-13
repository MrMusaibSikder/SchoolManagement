using SchoolERP.Application.Features.Section.DTOs;

namespace SchoolERP.Application.Features.Section.Interfaces;

/// <summary>
/// Business/service contract for Section records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface ISectionService
{
    /// <summary>Retrieves every Section record.</summary>
    Task<IReadOnlyList<SectionDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Section record by id, or null if it does not exist.</summary>
    Task<SectionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Section record.</summary>
    Task<SectionDto> CreateAsync(CreateSectionDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Section record.</summary>
    Task<SectionDto> UpdateAsync(int id, UpdateSectionDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Section record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
