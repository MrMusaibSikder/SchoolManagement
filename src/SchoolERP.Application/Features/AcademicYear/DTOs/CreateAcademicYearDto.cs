namespace SchoolERP.Application.Features.AcademicYear.DTOs;

/// <summary>Input model for creating a new AcademicYear record.</summary>
public class CreateAcademicYearDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
}
