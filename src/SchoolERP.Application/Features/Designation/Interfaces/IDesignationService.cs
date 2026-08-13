using SchoolERP.Application.Features.Designation.DTOs;

namespace SchoolERP.Application.Features.Designation.Interfaces;

/// <summary>
/// Business/service contract for Designation records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IDesignationService
{
    /// <summary>Retrieves every Designation record.</summary>
    Task<IReadOnlyList<DesignationDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single Designation record by id, or null if it does not exist.</summary>
    Task<DesignationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new Designation record.</summary>
    Task<DesignationDto> CreateAsync(CreateDesignationDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing Designation record.</summary>
    Task<DesignationDto> UpdateAsync(int id, UpdateDesignationDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing Designation record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
