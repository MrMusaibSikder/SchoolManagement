namespace SchoolERP.Application.Features.Guardian.DTOs;

/// <summary>Read model returned to clients for a Guardian record.</summary>
public class GuardianDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Occupation { get; set; }
}
