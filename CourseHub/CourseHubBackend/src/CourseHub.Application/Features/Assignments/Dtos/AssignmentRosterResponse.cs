using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments.Dtos
{
    public record AssignmentRosterResponse(
    Guid StudentId,
    string StudentName,
    string StudentCode,
    Guid? SubmissionId,
    string SubmissionStatus,
    string? SubmissionType,
    DateTime? SubmittedAt,
    bool IsLate,
    int? Marks,
    string? Feedback,
    DateTime? GradedAt);
}
