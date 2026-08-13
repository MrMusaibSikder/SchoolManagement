namespace SchoolERP.Application.Features.AcademicYear.DTOs;

/// <summary>Read model returned to clients for a AcademicYear record.</summary>
public class AcademicYearDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
}
