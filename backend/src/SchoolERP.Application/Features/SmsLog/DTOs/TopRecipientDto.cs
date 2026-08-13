namespace SchoolERP.Application.Features.SmsLog.DTOs;

/// <summary>A recipient ranked by how many SMS messages they received.</summary>
public class TopRecipientDto
{
    public string RecipientNumber { get; set; } = string.Empty;
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
    public int MessageCount { get; set; }
}
