namespace SchoolERP.Application.Features.ExamWeightSetup.DTOs;

/// <summary>Input model for creating a new exam weight setup, with its exam weight items, in one call.</summary>
public class CreateExamWeightSetupDto
{
    public int AcademicYearId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<CreateExamWeightItemDto> Items { get; set; } = new();
}
