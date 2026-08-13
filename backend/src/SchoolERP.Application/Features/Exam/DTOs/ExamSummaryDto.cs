using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Lightweight summary of a single exam, used in lists/dashboards instead of the full <see cref="ExamDto"/>.</summary>
public class ExamSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ExamTypeName { get; set; } = string.Empty;
    public string AcademicYearName { get; set; } = string.Empty;
    public ExamStatus Status { get; set; }
    public int TotalSchedules { get; set; }
    public DateTime? FirstExamDate { get; set; }
    public DateTime? LastExamDate { get; set; }
}
