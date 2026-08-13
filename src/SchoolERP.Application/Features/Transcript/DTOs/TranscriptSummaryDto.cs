using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>Top-level rollup shown at the head of a printed transcript.</summary>
public class TranscriptSummaryDto
{
    public int TotalExamsIncluded { get; set; }
    public int TotalAcademicYearsIncluded { get; set; }
    public decimal CGPA { get; set; }
    public decimal HighestYearGPA { get; set; }
    public decimal LowestYearGPA { get; set; }
    public bool OverallPassed { get; set; }

    /// <summary>Promotion status from the most recent academic year included.</summary>
    public PromotionStatus? LatestPromotionStatus { get; set; }
}
