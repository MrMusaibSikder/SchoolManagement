namespace SchoolERP.Application.Features.Guardian.DTOs;

/// <summary>Input model for creating a new Guardian record.</summary>
public class CreateGuardianDto
{
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }
}
