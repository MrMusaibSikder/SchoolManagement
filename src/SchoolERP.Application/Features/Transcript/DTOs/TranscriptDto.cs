namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>
/// Full, printable academic transcript for a student: every published exam
/// across every academic year (with subject-wise breakdown), each year's
/// weighted final outcome, GPA/position trends, an attendance summary, and an
/// overall CGPA. Contains only presentation-neutral data — no HTML/PDF markup
/// — so it can back a print view, a PDF export, or a JSON API response
/// identically.
/// </summary>
public class TranscriptDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;

    /// <summary>Top-level rollup for the transcript header.</summary>
    public TranscriptSummaryDto Summary { get; set; } = new();

    /// <summary>Every published exam result (with subject breakdown), in chronological order.</summary>
    public IReadOnlyList<TranscriptExamEntryDto> ExamHistory { get; set; } = Array.Empty<TranscriptExamEntryDto>();

    /// <summary>Every published academic-year final result (with subject breakdown and remarks), in chronological order.</summary>
    public IReadOnlyList<TranscriptYearSummaryDto> YearSummaries { get; set; } = Array.Empty<TranscriptYearSummaryDto>();

    /// <summary>Graph-ready GPA trend across academic years.</summary>
    public IReadOnlyList<GpaHistoryPointDto> GpaHistory { get; set; } = Array.Empty<GpaHistoryPointDto>();

    /// <summary>Graph-ready Class/Section/Merit position trend across academic years.</summary>
    public IReadOnlyList<PositionHistoryPointDto> PositionHistory { get; set; } = Array.Empty<PositionHistoryPointDto>();

    /// <summary>Attendance summary over the transcript's covered period (reused read-only from the existing Attendance module).</summary>
    public TranscriptAttendanceSummaryDto AttendanceSummary { get; set; } = new();

    /// <summary>UTC timestamp this transcript was generated (for print/PDF headers).</summary>
    public DateTime GeneratedAt { get; set; }
}
