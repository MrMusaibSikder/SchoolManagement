using Microsoft.AspNetCore.Http;

namespace SchoolERP.Application.Features.School.DTOs;

/// <summary>Input model for updating an existing School record.</summary>

public class UpdateSchoolDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? EIIN { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public IFormFile? LogoFile { get; set; }
}


