using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Assignments.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments
{
    public interface IAssignmentSubmissionService
    {
        Task<SubmissionResponse> SubmitTextAsync(
            Guid assignmentId,
            Guid studentId,
            SubmitTextAssignmentRequest request,
            CancellationToken cancellationToken = default);

        Task<SubmissionResponse> GradeAsync(
            Guid submissionId,
            Guid? teacherId,
            GradeSubmissionRequest request,
            CancellationToken cancellationToken = default);

        Task<SubmissionResponse> GetMySubmissionAsync(
            Guid assignmentId,
            Guid studentId,
            CancellationToken cancellationToken = default);

        Task<SubmissionResponse> SubmitFileAsync(
    Guid assignmentId,
    Guid studentId,
    Stream fileStream,
    string fileName,
    string contentType,
    CancellationToken cancellationToken = default);

        Task<IReadOnlyList<AssignmentRosterResponse>> GetRosterAsync(
    Guid assignmentId,
    CancellationToken cancellationToken = default);

        Task<FileDownloadResult> DownloadFileAsync(
    Guid submissionId,
    Guid? currentUserId,
    CancellationToken cancellationToken = default);
    }




}
