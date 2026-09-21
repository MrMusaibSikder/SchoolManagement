using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Assignments.Dtos;
using CourseHub.Application.Features.Enrollments.Dtos;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Me;

public class MeService : IMeService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IStudentRepository _studentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ISubmissionRepository _submissionRepository;

    public MeService(
        IStudentRepository studentRepository,
        IEnrollmentRepository enrollmentRepository,
        IAssignmentRepository assignmentRepository,
        ISubmissionRepository submissionRepository)
    {
        _studentRepository = studentRepository;
        _enrollmentRepository = enrollmentRepository;
        _assignmentRepository = assignmentRepository;
        _submissionRepository = submissionRepository;
    }

    public async Task<PagedResult<EnrollmentResponse>> GetMyEnrollmentsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByUserIdAsync(
            userId,
            cancellationToken)
            ?? throw new NotFoundException(
                "You don't have a student profile yet — ask an administrator to create one for you.");

        var normalizedPage = Math.Max(page, 1);

        var normalizedPageSize = Math.Clamp(
            pageSize <= 0 ? DefaultPageSize : pageSize,
            1,
            MaxPageSize);

        var (items, totalCount) = await _enrollmentRepository.SearchAsync(
            student.Id,
            null,
            null,
            normalizedPage,
            normalizedPageSize,
            cancellationToken);

        var responses = items
            .Select(e => new EnrollmentResponse(
                e.Id,
                e.StudentId,
                e.BatchId,
                e.EnrollmentDate,
                e.Status.ToString(),
                e.CreatedAt,
                e.UpdatedAt))
            .ToList();

        return new PagedResult<EnrollmentResponse>(
            responses,
            totalCount,
            normalizedPage,
            normalizedPageSize);
    }

    public async Task<IReadOnlyList<MyAssignmentResponse>> GetMyAssignmentsAsync(
     Guid userId,
     CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByUserIdAsync(
            userId,
            cancellationToken)
            ?? throw new NotFoundException(
                "You don't have a student profile yet — ask an administrator to create one for you.");

        // Get all courses where the student is actively or
        // previously completed enrolled.
        var courseIds = await _enrollmentRepository
            .GetEnrolledCourseIdsByStudentIdAsync(
                student.Id,
                cancellationToken);

        if (courseIds.Count == 0)
            return Array.Empty<MyAssignmentResponse>();

        // Get all active assignments from those courses.
        var assignments = await _assignmentRepository
            .GetActiveByCourseIdsAsync(
                courseIds,
                cancellationToken);

        if (assignments.Count == 0)
            return Array.Empty<MyAssignmentResponse>();

        var responses = new List<MyAssignmentResponse>();

        foreach (var assignment in assignments)
        {
            var submission =
                await _submissionRepository.GetByAssignmentAndStudentAsync(
                    assignment.Id,
                    student.Id,
                    cancellationToken);

            responses.Add(new MyAssignmentResponse(
                assignment.Id,
                assignment.CourseId,
                assignment.CreatedByTeacherId,
                assignment.Title,
                assignment.Description,
                assignment.MaxMarks,
                assignment.DueDate,
                assignment.IsActive,
                assignment.CreatedAt,
                assignment.UpdatedAt,
                submission is null ? null : ToResponse(submission)));
        }

        return responses;
    }

    public async Task<PagedResult<SubmissionResponse>> GetMySubmissionsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByUserIdAsync(
            userId,
            cancellationToken)
            ?? throw new NotFoundException(
                "You don't have a student profile yet — ask an administrator to create one for you.");

        var normalizedPage = Math.Max(page, 1);

        var normalizedPageSize = Math.Clamp(
            pageSize <= 0 ? DefaultPageSize : pageSize,
            1,
            MaxPageSize);

        var (items, totalCount) =
            await _submissionRepository.GetByStudentAsync(
                student.Id,
                normalizedPage,
                normalizedPageSize,
                cancellationToken);

        var responses = items
            .Select(ToResponse)
            .ToList();

        return new PagedResult<SubmissionResponse>(
            responses,
            totalCount,
            normalizedPage,
            normalizedPageSize);
    }

    private static SubmissionResponse ToResponse(
        Domain.Entities.AssignmentSubmission submission)
    {
        return new SubmissionResponse(
            submission.Id,
            submission.AssignmentId,
            submission.StudentId,
            submission.SubmissionType.ToString(),
            submission.TextContent,
            submission.FilePath,
            submission.OriginalFileName,
            submission.SubmittedAt,
            submission.IsLate,
            submission.Marks,
            submission.Feedback,
            submission.GradedAt,
            submission.GradedByTeacherId,
            submission.Status.ToString());
    }
}
