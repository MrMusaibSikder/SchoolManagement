namespace SchoolERP.Application.Features.GradeSetup.DTOs;

/// <summary>Input model for creating a new grade band.</summary>
public class CreateGradeSetupDto
{
    public int AcademicYearId { get; set; }
    public string GradeName { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public decimal MinMarks { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal MinPercentage { get; set; }
    public decimal MaxPercentage { get; set; }
    public bool IsFail { get; set; }
    public int DisplayOrder { get; set; }
}
