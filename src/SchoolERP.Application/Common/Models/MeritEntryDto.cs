namespace SchoolERP.Application.Common.Models;

/// <summary>A single row in a merit/rank list (class, section, school-wide, or final-result merit list).</summary>
public class MeritEntryDto
{
    public int Position { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; }
    public decimal GPA { get; set; }
    public string Grade { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
}
