using SchoolERP.Application.Features.ExamType.DTOs;

namespace SchoolERP.Application.Features.ExamType.Interfaces;

/// <summary>
/// Business/service contract for ExamType records. Services return DTOs only
/// and encapsulate all business rules for this feature.
/// </summary>
public interface IExamTypeService
{
    /// <summary>Retrieves every ExamType record.</summary>
    Task<IReadOnlyList<ExamTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single ExamType record by id, or null if it does not exist.</summary>
    Task<ExamTypeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Creates a new ExamType record.</summary>
    Task<ExamTypeDto> CreateAsync(CreateExamTypeDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing ExamType record.</summary>
    Task<ExamTypeDto> UpdateAsync(int id, UpdateExamTypeDto request, CancellationToken cancellationToken = default);

    /// <summary>Soft-deletes an existing ExamType record.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
