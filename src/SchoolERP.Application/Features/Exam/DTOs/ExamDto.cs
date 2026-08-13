using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Read model returned to clients for a Exam record.</summary>
public class ExamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ExamTypeId { get; set; }
    public string? ExamTypeName { get; set; }
    public int AcademicYearId { get; set; }
    public string? AcademicYearName { get; set; }

    /// <summary>Current lifecycle state of the exam.</summary>
    public ExamStatus Status { get; set; }
}
