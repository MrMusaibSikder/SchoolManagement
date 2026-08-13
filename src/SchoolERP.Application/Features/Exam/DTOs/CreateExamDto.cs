namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Input model for creating a new Exam record.</summary>
public class CreateExamDto
{
    public string Name { get; set; } = string.Empty;
    public int ExamTypeId { get; set; }
    public int AcademicYearId { get; set; }
}
