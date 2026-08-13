namespace SchoolERP.Application.Features.Transcript.DTOs;

/// <summary>A single graph-ready (label, GPA) point for a transcript's GPA trend chart.</summary>
public class GpaHistoryPointDto
{
    public string Label { get; set; } = string.Empty;
    public decimal GPA { get; set; }
}
