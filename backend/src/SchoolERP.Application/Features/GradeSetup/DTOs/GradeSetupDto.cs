namespace SchoolERP.Application.Features.GradeSetup.DTOs;

/// <summary>Read model returned to clients for a GradeSetup (grade band) record.</summary>
public class GradeSetupDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string GradeName { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public decimal MinMarks { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal MinPercentage { get; set; }
    public decimal MaxPercentage { get; set; }
    public bool IsFail { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}
