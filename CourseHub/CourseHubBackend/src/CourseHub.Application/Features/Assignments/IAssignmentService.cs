using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Features.Assignments.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments
{
    public interface IAssignmentService
    {
        Task<PagedResult<AssignmentResponse>> SearchAsync(
            Guid? courseId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<AssignmentResponse> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<AssignmentResponse> CreateAsync(
            CreateAssignmentRequest request,
            CancellationToken cancellationToken = default);

        Task<AssignmentResponse> UpdateAsync(
            Guid id,
            UpdateAssignmentRequest request,
            CancellationToken cancellationToken = default);

        Task<AssignmentResponse> ActivateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<AssignmentResponse> DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<RosterEntryResponse>> GetRosterAsync(Guid assignmentId, CancellationToken cancellationToken = default);

        Task<SubmissionResponse> GradeSubmissionAsync(Guid submissionId, GradeSubmissionRequest request, Guid graderUserId, CancellationToken cancellationToken = default);
    }
}
