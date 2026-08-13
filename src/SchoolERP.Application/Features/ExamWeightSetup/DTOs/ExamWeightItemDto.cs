namespace SchoolERP.Application.Features.ExamWeightSetup.DTOs;

/// <summary>Read model for a single exam's weight within an <see cref="ExamWeightSetupDto"/>.</summary>
public class ExamWeightItemDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public decimal WeightPercentage { get; set; }
}
