namespace SchoolERP.Application.Features.ExamWeightSetup.DTOs;

/// <summary>Input model for adding a single exam weight item to an existing setup.</summary>
public class AddExamWeightItemDto
{
    public int ExamWeightSetupId { get; set; }
    public int ExamId { get; set; }
    public decimal WeightPercentage { get; set; }
}
