namespace SchoolERP.Application.Common.Helpers;

/// <summary>
/// Converts a percentage score into a letter grade and grade point, using the
/// standard Bangladesh secondary-education GPA scale. Centralized here so
/// every part of the Result module (subject marks, exam aggregates, final
/// weighted results) grades consistently from one place.
/// </summary>
public static class GradeCalculator
{
    private static readonly (decimal MinPercentage, string Grade, decimal GradePoint)[] Scale =
    {
        (80m, "A+", 5.00m),
        (70m, "A", 4.00m),
        (60m, "A-", 3.50m),
        (50m, "B", 3.00m),
        (40m, "C", 2.00m),
        (33m, "D", 1.00m),
        (0m, "F", 0.00m)
    };

    /// <summary>
    /// Computes the (Grade, GradePoint) pair for a percentage score, honoring
    /// an explicit pass threshold: scoring below <paramref name="passPercentage"/>
    /// always yields an F/0.00 regardless of where it would otherwise fall on
    /// the scale (e.g. a subject with a high FullMarks but a strict PassMarks).
    /// </summary>
    public static (string Grade, decimal GradePoint) Calculate(decimal percentage, decimal passPercentage)
    {
        if (percentage < passPercentage)
        {
            return ("F", 0.00m);
        }

        foreach (var (minPercentage, grade, gradePoint) in Scale)
        {
            if (percentage >= minPercentage)
            {
                return (grade, gradePoint);
            }
        }

        return ("F", 0.00m);
    }

    /// <summary>Computes the letter grade label for an average grade point (e.g. across subjects), mapping it onto the same scale. Returns the average itself as the grade point (not snapped to the bucket).</summary>
    public static (string Grade, decimal GradePoint) FromAverageGradePoint(decimal averageGradePoint)
    {
        foreach (var (_, grade, gradePoint) in Scale)
        {
            if (averageGradePoint >= gradePoint)
            {
                return (grade, averageGradePoint);
            }
        }

        return ("F", averageGradePoint);
    }
}
