namespace SchoolERP.Application.Features.ExamResult.DTOs;

/// <summary>Subject-wise breakdown row within a student's result for one exam. Projected from Result (mark entry) rows, not its own table.</summary>
public class ExamResultDetailDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public decimal MarksObtained { get; set; }
    public decimal GraceMarks { get; set; }
    public int FullMarks { get; set; }
    public int PassMarks { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public bool IsPassed { get; set; }

    /// <summary>Whether this subject was optional for the student's class.</summary>
    public bool IsOptional { get; set; }
}
