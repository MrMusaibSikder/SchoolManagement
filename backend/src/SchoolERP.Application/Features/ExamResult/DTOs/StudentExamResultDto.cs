namespace SchoolERP.Application.Features.ExamResult.DTOs;

/// <summary>Full "mark sheet" view for one student's result in one exam: the aggregate plus every subject's breakdown.</summary>
public class StudentExamResultDto
{
    public ExamResultDto Summary { get; set; } = new();
    public IReadOnlyList<ExamResultDetailDto> Subjects { get; set; } = Array.Empty<ExamResultDetailDto>();
}
