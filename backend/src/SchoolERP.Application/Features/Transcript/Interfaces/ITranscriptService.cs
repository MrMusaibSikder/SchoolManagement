using SchoolERP.Application.Features.Transcript.DTOs;

namespace SchoolERP.Application.Features.Transcript.Interfaces;

/// <summary>
/// Business/service contract for generating student transcripts. Composes
/// data purely from the existing <c>IExamResultRepository</c> and
/// <c>IFinalResultRepository</c> (via the Unit of Work) — introduces no new
/// storage of its own. Only published results are included, consistent with
/// the "only published results are visible" rule used elsewhere.
/// </summary>
public interface ITranscriptService
{
    /// <summary>
    /// Builds the full multi-academic-year transcript for a student: every
    /// published exam result plus every published year-end final result, and
    /// an overall CGPA.
    /// </summary>
    Task<TranscriptDto> GetStudentTranscriptAsync(int studentId, CancellationToken cancellationToken = default);

    /// <summary>Builds a transcript scoped to a single academic year (its exams plus that year's final result, if published).</summary>
    Task<TranscriptDto> GetAcademicYearTranscriptAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default);

  
}
