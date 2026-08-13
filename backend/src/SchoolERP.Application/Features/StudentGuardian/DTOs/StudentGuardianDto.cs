namespace SchoolERP.Application.Features.StudentGuardian.DTOs;

/// <summary>Data transfer object representing a Student-Guardian association.</summary>
public class StudentGuardianDto
{
    public int GuardianId { get; set; }

    public string GuardianName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Relation { get; set; } = string.Empty;
}
