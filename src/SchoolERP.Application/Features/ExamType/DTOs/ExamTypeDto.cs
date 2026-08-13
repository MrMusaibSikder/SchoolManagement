namespace SchoolERP.Application.Features.ExamType.DTOs;

/// <summary>Read model returned to clients for a ExamType record.</summary>
public class ExamTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
