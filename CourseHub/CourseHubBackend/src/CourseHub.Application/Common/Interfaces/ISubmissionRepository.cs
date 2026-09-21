using CourseHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Common.Interfaces
{
    public interface ISubmissionRepository
    {
        Task<AssignmentSubmission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// The unique-index lookup this whole feature depends on — one
        /// submission per (assignment, student). Used both to detect a
        /// resubmit (replace existing) and to block grading twice.
        /// </summary>
        Task<AssignmentSubmission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Every submission for a single assignment — the grading screen's
        /// data source (paired with the assignment's enrolled-student roster
        /// at the service layer, to also show who *hasn't* submitted yet).
        /// </summary>
        Task<IReadOnlyList<AssignmentSubmission>> GetByAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Every submission a single student has ever made, across any
        /// assignment — MeService.GetMySubmissionsAsync's data source.
        /// </summary>
        Task<(IReadOnlyList<AssignmentSubmission> Items, int TotalCount)> GetByStudentAsync(
            Guid studentId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task AddAsync(AssignmentSubmission submission, CancellationToken cancellationToken = default);
    }
}
