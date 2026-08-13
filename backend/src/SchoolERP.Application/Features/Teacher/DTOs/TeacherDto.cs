namespace SchoolERP.Application.Features.Teacher.DTOs;

/// <summary>Read model returned to clients for a Teacher record.</summary>
public class TeacherDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
}
