using SchoolERP.Application.Common.Helpers;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Default <see cref="IGradeLookupService"/> implementation. Consults the
/// active <see cref="GradeSetup"/> bands configured for the given academic
/// year; when none are configured, falls back to the built-in standard
/// Bangladesh GPA scale (<see cref="GradeCalculator"/>) so every existing
/// calculation keeps working exactly as before for years without custom
/// grading. The synchronous overloads let calculation-heavy services
/// (looping over many students/subjects) fetch the bands once and reuse them,
/// avoiding N+1 queries.
/// </summary>
public class GradeLookupService : IGradeLookupService
{
    private readonly IUnitOfWork _unitOfWork;

    public GradeLookupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GradeSetup>> GetBandsAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.GradeSetupRepository.GetActiveByAcademicYearAsync(academicYearId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(string Grade, decimal GradePoint, bool IsFail)> ResolveByPercentageAsync(
        int academicYearId,
        decimal percentage,
        decimal passPercentage,
        CancellationToken cancellationToken = default)
    {
        var bands = await GetBandsAsync(academicYearId, cancellationToken);
        return ResolveByPercentage(bands, percentage, passPercentage);
    }

    /// <inheritdoc />
    public async Task<(string Grade, decimal GradePoint)> ResolveByGradePointAsync(
        int academicYearId,
        decimal averageGradePoint,
        CancellationToken cancellationToken = default)
    {
        var bands = await GetBandsAsync(academicYearId, cancellationToken);
        return ResolveByGradePoint(bands, averageGradePoint);
    }

    /// <inheritdoc />
    public (string Grade, decimal GradePoint, bool IsFail) ResolveByPercentage(
        IReadOnlyList<GradeSetup> bands,
        decimal percentage,
        decimal passPercentage)
    {
        if (bands.Count > 0)
        {
            // Below the pass threshold always fails, regardless of which band the raw percentage would otherwise land in.
            if (percentage < passPercentage)
            {
                var failBand = bands.FirstOrDefault(b => b.IsFail);
                return failBand is not null
                    ? (failBand.GradeName, failBand.GradePoint, true)
                    : ("F", 0.00m, true);
            }

            var match = bands
                .Where(b => !b.IsFail)
                .OrderBy(b => b.DisplayOrder)
                .FirstOrDefault(b => percentage >= b.MinPercentage && percentage <= b.MaxPercentage);

            if (match is not null)
            {
                return (match.GradeName, match.GradePoint, false);
            }

            // Configured but no band covers this percentage (gap in setup): fall through to the static scale as a safe default.
        }

        var (grade, gradePoint) = GradeCalculator.Calculate(percentage, passPercentage);
        return (grade, gradePoint, grade == "F");
    }

    /// <inheritdoc />
    public (string Grade, decimal GradePoint) ResolveByGradePoint(
        IReadOnlyList<GradeSetup> bands,
        decimal averageGradePoint)
    {
        if (bands.Count > 0)
        {
            var match = bands
                .Where(b => !b.IsFail)
                .OrderByDescending(b => b.GradePoint)
                .FirstOrDefault(b => averageGradePoint >= b.GradePoint);

            if (match is not null)
            {
                return (match.GradeName, averageGradePoint);
            }

            var failBand = bands.FirstOrDefault(b => b.IsFail);
            if (failBand is not null)
            {
                return (failBand.GradeName, averageGradePoint);
            }
        }

        return GradeCalculator.FromAverageGradePoint(averageGradePoint);
    }
}
