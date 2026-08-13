namespace SchoolERP.Application.Common.Helpers;

/// <summary>
/// Centralizes the "optional subject" GPA rule (standard Bangladesh "4th
/// subject" convention) so <c>ExamResultService</c> and
/// <c>FinalResultService</c> apply it identically. Optional subjects never
/// count toward the fail count and contribute only a capped bonus grade
/// point on top of the mandatory-subject average.
/// </summary>
public static class ResultGradingRules
{
    /// <summary>Grade point an optional subject must exceed before it contributes any bonus (below this, it adds nothing).</summary>
    public const decimal OptionalSubjectBonusThreshold = 2.00m;

    /// <summary>The configured maximum possible GPA. The result of <see cref="CalculateGpaWithOptionalBonus"/> never exceeds this.</summary>
    public const decimal MaxGpa = 5.00m;

    /// <summary>
    /// Computes the overall grade point as:
    /// (sum of mandatory subject grade points + capped optional-subject bonus) / count of mandatory subjects,
    /// never exceeding <see cref="MaxGpa"/>. If there are no mandatory subjects
    /// (a data setup edge case), falls back to the plain average of every
    /// supplied grade point.
    /// </summary>
    public static decimal CalculateGpaWithOptionalBonus(
        IEnumerable<decimal> mandatoryGradePoints,
        IEnumerable<decimal> optionalGradePoints)
    {
        var mandatory = mandatoryGradePoints.ToList();
        var optional = optionalGradePoints.ToList();

        if (mandatory.Count == 0)
        {
            var all = mandatory.Concat(optional).ToList();
            return all.Count == 0 ? 0m : Math.Round(Math.Min(all.Average(), MaxGpa), 2);
        }

        var mandatorySum = mandatory.Sum();
        var optionalBonus = optional.Sum(gp => Math.Max(0m, gp - OptionalSubjectBonusThreshold));

        var rawGpa = (mandatorySum + optionalBonus) / mandatory.Count;

        return Math.Round(Math.Min(rawGpa, MaxGpa), 2);
    }
}
