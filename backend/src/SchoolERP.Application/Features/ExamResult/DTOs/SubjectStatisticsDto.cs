namespace SchoolERP.Application.Features.ExamResult.DTOs;

/// <summary>Aggregate statistics for a single subject within a single exam.</summary>
public class SubjectStatisticsDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TotalStudents { get; set; }
    public decimal HighestMarks { get; set; }
    public decimal LowestMarks { get; set; }
    public decimal AverageMarks { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public decimal PassRate { get; set; }
}
