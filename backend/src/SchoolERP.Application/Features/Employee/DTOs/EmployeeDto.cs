namespace SchoolERP.Application.Features.Employee.DTOs;

/// <summary>Read model returned to clients for a Employee record.</summary>
public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime JoiningDate { get; set; }
    public bool IsActive { get; set; }
    public string? EmployeePhoto {  get; set; }
    public int DesignationId { get; set; }
    public int? UserId { get; set; }
}
