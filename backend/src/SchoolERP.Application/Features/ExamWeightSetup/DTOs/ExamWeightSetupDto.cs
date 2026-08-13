namespace SchoolERP.Application.Features.ExamWeightSetup.DTOs;

/// <summary>Read model returned to clients for an ExamWeightSetup record, including its items.</summary>
public class ExamWeightSetupDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    /// <summary>Sum of every item's WeightPercentage. Must equal 100 for the setup to be activated.</summary>
    public decimal TotalWeight { get; set; }

    public IReadOnlyList<ExamWeightItemDto> Items { get; set; } = Array.Empty<ExamWeightItemDto>();
}
