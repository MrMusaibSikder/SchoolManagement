using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments.Dtos
{
    /// <summary>
    /// One row of the grading screen's roster — every enrolled student for
    /// the assignment's course, whether they've submitted or not. Submission
    /// is null when the student hasn't submitted yet, which the grading
    /// screen shows as "Not submitted" instead of just omitting the row
    /// entirely (a teacher needs to see who's missing, not just who's done).
    /// </summary>
    public record RosterEntryResponse(
        Guid StudentId,
        string StudentFullName,
        SubmissionResponse? Submission);
}
