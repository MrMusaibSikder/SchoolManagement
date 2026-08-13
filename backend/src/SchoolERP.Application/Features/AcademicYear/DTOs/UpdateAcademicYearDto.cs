namespace SchoolERP.Application.Features.AcademicYear.DTOs;

/// <summary>Input model for updating an existing AcademicYear record.</summary>
public class UpdateAcademicYearDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
}
