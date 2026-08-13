namespace SchoolERP.Application.Features.ExamResult.DTOs;

/// <summary>Number (and percentage) of students achieving a given grade in an exam.</summary>
public class GradeDistributionItemDto
{
    public string Grade { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}
