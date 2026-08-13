namespace SchoolERP.Application.Features.Exam.DTOs;

/// <summary>Input model for updating an existing Exam record.</summary>
public class UpdateExamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ExamTypeId { get; set; }
    public int AcademicYearId { get; set; }
}
