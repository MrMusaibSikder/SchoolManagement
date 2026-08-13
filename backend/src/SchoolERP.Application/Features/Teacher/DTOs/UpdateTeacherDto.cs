namespace SchoolERP.Application.Features.Teacher.DTOs;

/// <summary>Input model for updating an existing Teacher record.</summary>
public class UpdateTeacherDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
}
