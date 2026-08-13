using Microsoft.AspNetCore.Http;

namespace SchoolERP.Application.Features.School.DTOs;

/// <summary>Input model for creating a new School record.</summary>
public class CreateSchoolDto
{
    public string Name { get; set; } = string.Empty;
    public string? EIIN { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public IFormFile? LogoFile { get; set; }
}
