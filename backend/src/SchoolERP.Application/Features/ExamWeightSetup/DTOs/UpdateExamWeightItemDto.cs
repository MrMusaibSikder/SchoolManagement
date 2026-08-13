namespace SchoolERP.Application.Features.ExamWeightSetup.DTOs;

/// <summary>Input model for changing a single exam weight item's percentage.</summary>
public class UpdateExamWeightItemDto
{
    public int Id { get; set; }
    public decimal WeightPercentage { get; set; }
}
