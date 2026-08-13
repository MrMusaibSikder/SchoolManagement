namespace SchoolERP.Application.Features.ExamType.DTOs;

/// <summary>Input model for updating an existing ExamType record.</summary>
public class UpdateExamTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
