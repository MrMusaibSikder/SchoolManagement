using Microsoft.AspNetCore.Http;

namespace SchoolERP.Application.Features.School.DTOs;

/// <summary>Read model returned to clients for a School record.</summary>
public class SchoolDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? EIIN { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Logo { get; set; }
}
