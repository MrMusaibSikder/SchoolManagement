namespace SchoolERP.Application.Common.Interfaces.Services;

/// <summary>
/// Resolves a percentage (or average grade point) into a (Grade, GradePoint,
/// IsFail) result for a given academic year. Consults the year's configured
/// <see cref="SchoolERP.Domain.Entities.GradeSetup"/> bands first (see
/// <c>Features.GradeSetup</c>); when no bands are configured for that year,
/// falls back to the built-in standard scale (<c>GradeCalculator</c>) so
/// every existing exam/final result calculation keeps working unchanged.
/// Used by ResultService, ExamResultService and FinalResultService so
/// grading policy can be changed per academic year without a code deployment.
/// </summary>
public interface IGradeLookupService
{
    /// <summary>Resolves a subject/exam-level percentage score against a pass threshold into a graded outcome.</summary>
    Task<(string Grade, decimal GradePoint, bool IsFail)> ResolveByPercentageAsync(
        int academicYearId,
        decimal percentage,
        decimal passPercentage,
        CancellationToken cancellationToken = default);

    /// <summary>Resolves an average grade point (e.g. across subjects) into its letter grade label.</summary>
    Task<(string Grade, decimal GradePoint)> ResolveByGradePointAsync(
        int academicYearId,
        decimal averageGradePoint,
        CancellationToken cancellationToken = default);

    /// <summary>Fetches the active grade bands for an academic year once, for reuse across many synchronous lookups in a loop (avoids N+1 queries).</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.GradeSetup>> GetBandsAsync(
        int academicYearId,
        CancellationToken cancellationToken = default);

    /// <summary>Synchronous, pre-fetched-bands version of <see cref="ResolveByPercentageAsync"/>, for use inside loops/LINQ.</summary>
    (string Grade, decimal GradePoint, bool IsFail) ResolveByPercentage(
        IReadOnlyList<SchoolERP.Domain.Entities.GradeSetup> bands,
        decimal percentage,
        decimal passPercentage);

    /// <summary>Synchronous, pre-fetched-bands version of <see cref="ResolveByGradePointAsync"/>, for use inside loops/LINQ.</summary>
    (string Grade, decimal GradePoint) ResolveByGradePoint(
        IReadOnlyList<SchoolERP.Domain.Entities.GradeSetup> bands,
        decimal averageGradePoint);
}
