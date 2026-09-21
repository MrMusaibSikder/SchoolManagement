using CourseHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments.Dtos
{
    public record SubmissionResponse(
     Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    string SubmissionType,
    string? TextContent,
    string? FilePath,
    string? OriginalFileName,
    DateTime SubmittedAt,
    bool IsLate,
    int? Marks,
    string? Feedback,
    DateTime? GradedAt,
    Guid? GradedByTeacherId,
    string Status);
}
