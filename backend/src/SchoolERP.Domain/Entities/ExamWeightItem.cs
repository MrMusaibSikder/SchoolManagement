using SchoolERP.Domain.Common;

namespace SchoolERP.Domain.Entities;

/// <summary>A single exam's contribution weight (percentage) within an <see cref="ExamWeightSetup"/>.</summary>
public class ExamWeightItem : BaseEntity
{
    public int ExamWeightSetupId { get; set; }
    public ExamWeightSetup? ExamWeightSetup { get; set; }

    public int ExamId { get; set; }
    public Exam? Exam { get; set; }

    /// <summary>Weight of this exam's marks toward the final result, in percent (0-100). All items in a setup must sum to exactly 100.</summary>
    public decimal WeightPercentage { get; set; }
}
