namespace SchoolERP.Application.Features.ExamResult.DTOs;

/// <summary>One student's row within a <see cref="TabulationSheetDto"/>: marks per subject plus totals.</summary>
public class TabulationRowDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string RollNo { get; set; } = string.Empty;

    /// <summary>Subject name -> marks obtained (including grace), in the same order as <see cref="TabulationSheetDto.SubjectNames"/>.</summary>
    public Dictionary<string, decimal> SubjectMarks { get; set; } = new();

    public decimal TotalMarks { get; set; }
    public decimal GPA { get; set; }
    public string Grade { get; set; } = string.Empty;
    public bool IsPassed { get; set; }
    public int? ClassPosition { get; set; }
}

/// <summary>Full class tabulation sheet for one exam: every subject as a column, every student as a row.</summary>
public class TabulationSheetDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    public IReadOnlyList<string> SubjectNames { get; set; } = Array.Empty<string>();
    public IReadOnlyList<TabulationRowDto> Rows { get; set; } = Array.Empty<TabulationRowDto>();
}
