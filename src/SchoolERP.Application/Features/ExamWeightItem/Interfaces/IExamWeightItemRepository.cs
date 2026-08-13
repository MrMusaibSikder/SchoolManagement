using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.ExamWeightItem.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.ExamWeightItem"/>
/// entities. Extends the generic repository with data access members needed
/// for duplicate-exam checks and setup total computation. Contains database
/// operations only.
/// </summary>
public interface IExamWeightItemRepository : IGenericRepository<SchoolERP.Domain.Entities.ExamWeightItem>
{
    /// <summary>Gets every item belonging to a setup, with Exam eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamWeightItem>> GetBySetupAsync(
        int examWeightSetupId,
        CancellationToken cancellationToken = default);

    /// <summary>Checks whether the given exam already has a weight item within the setup.</summary>
    Task<bool> ExamExistsInSetupAsync(
        int examWeightSetupId,
        int examId,
        int? excludeId,
        CancellationToken cancellationToken = default);

    /// <summary>Sums every item's WeightPercentage within a setup.</summary>
    Task<decimal> GetTotalWeightAsync(
        int examWeightSetupId,
        CancellationToken cancellationToken = default);
}
