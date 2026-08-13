namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>A single graph-ready position-trend point (per academic year) for a transcript.</summary>
public class PositionHistoryPointDto
{
    public string AcademicYearName { get; set; } = string.Empty;
    public int? ClassPosition { get; set; }
    public int? SectionPosition { get; set; }
    public int? MeritPosition { get; set; }
}
