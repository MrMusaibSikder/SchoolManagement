namespace SchoolERP.Application.Features.Teacher.DTOs;

/// <summary>Input model for creating a new Teacher record.</summary>
public class CreateTeacherDto
{
    public int EmployeeId { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
}
