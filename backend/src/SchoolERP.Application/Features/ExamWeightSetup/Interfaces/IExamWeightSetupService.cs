using SchoolERP.Application.Features.ExamWeightSetup.DTOs;

namespace SchoolERP.Application.Features.ExamWeightSetup.Interfaces;

/// <summary>
/// Business/service contract for exam weight setups and their items. A setup
/// groups the percentage contribution of each exam (e.g. Mid Term 1 = 20%)
/// toward a student's <see cref="SchoolERP.Domain.Entities.FinalResult"/> for
/// an academic year. Encapsulates the "weights must total 100% to activate"
/// rule and the "only one active setup per academic year" rule.
/// </summary>
public interface IExamWeightSetupService
{
    /// <summary>Retrieves every exam weight setup (enriched with names).</summary>
    Task<IReadOnlyList<ExamWeightSetupDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Retrieves a single exam weight setup by id, or null if it does not exist.</summary>
    Task<ExamWeightSetupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Retrieves every setup (active and inactive — version history) for an academic year.</summary>
    Task<IReadOnlyList<ExamWeightSetupDto>> GetByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Retrieves the currently active setup for an academic year, or null if none is active.</summary>
    Task<ExamWeightSetupDto?> GetActiveByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Creates a new (inactive) weight setup with its items in one call. Each exam may appear at most once.</summary>
    Task<ExamWeightSetupDto> CreateAsync(CreateExamWeightSetupDto request, CancellationToken cancellationToken = default);

    /// <summary>Renames an existing weight setup.</summary>
    Task<ExamWeightSetupDto> UpdateAsync(int id, UpdateExamWeightSetupDto request, CancellationToken cancellationToken = default);

    /// <summary>Deletes an inactive weight setup. Active setups cannot be deleted (deactivate first).</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a setup, making it the effective weighting for its academic
    /// year. Requires the setup's items to sum to exactly 100%. Deactivates
    /// any other setup previously active for the same academic year
    /// (versioning: the old one is kept, just no longer active).
    /// </summary>
    Task<ExamWeightSetupDto> ActivateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Deactivates a setup without activating a replacement.</summary>
    Task<ExamWeightSetupDto> DeactivateAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Adds a single exam weight item to an existing (inactive) setup.</summary>
    Task<ExamWeightSetupDto> AddItemAsync(AddExamWeightItemDto request, CancellationToken cancellationToken = default);

    /// <summary>Updates a single exam weight item's percentage. Not permitted once the parent setup is active.</summary>
    Task<ExamWeightSetupDto> UpdateItemAsync(int itemId, UpdateExamWeightItemDto request, CancellationToken cancellationToken = default);

    /// <summary>Removes a single exam weight item from its setup. Not permitted once the parent setup is active.</summary>
    Task<ExamWeightSetupDto> RemoveItemAsync(int itemId, CancellationToken cancellationToken = default);
}
