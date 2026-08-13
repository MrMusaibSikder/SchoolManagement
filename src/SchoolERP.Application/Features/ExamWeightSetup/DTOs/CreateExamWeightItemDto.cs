namespace SchoolERP.Application.Features.ExamWeightSetup.DTOs;

/// <summary>A single exam's weight, supplied when creating a new <see cref="CreateExamWeightSetupDto"/>.</summary>
public class CreateExamWeightItemDto
{
    public int ExamId { get; set; }
    public decimal WeightPercentage { get; set; }
}
