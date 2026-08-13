using Microsoft.AspNetCore.Http;

namespace SchoolERP.Application.Features.Employee.DTOs;

/// <summary>Input model for creating a new Employee record.</summary>
public class CreateEmployeeDto
{
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime JoiningDate { get; set; }
    public bool IsActive { get; set; }
    public int DesignationId { get; set; }
    public IFormFile? EmployeePhotoFile { get; set; }
    public int? UserId { get; set; }
}
