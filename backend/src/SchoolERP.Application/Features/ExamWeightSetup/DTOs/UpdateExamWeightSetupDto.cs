namespace SchoolERP.Application.Features.ExamWeightSetup.DTOs;

/// <summary>Input model for renaming an existing exam weight setup. Items are managed separately.</summary>
public class UpdateExamWeightSetupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
