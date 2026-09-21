using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Features.Assignments.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments
{
    public class AssignmentService : IAssignmentService
    {
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 100;

        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;

        public AssignmentService(
            IAssignmentRepository assignmentRepository,
            ICourseRepository courseRepository, ISubmissionRepository submissionRepository,
            IEnrollmentRepository enrollmentRepository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository,
            IUnitOfWork unitOfWork)
        {
            _assignmentRepository = assignmentRepository;
            _courseRepository = courseRepository;
            _submissionRepository = submissionRepository;
            _enrollmentRepository = enrollmentRepository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<AssignmentResponse>> SearchAsync(
            Guid? courseId,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var normalizedPage = Math.Max(page, 1);

            var normalizedPageSize = Math.Clamp(
                pageSize <= 0 ? DefaultPageSize : pageSize,
                1,
                MaxPageSize);

            var (items, totalCount) =
                await _assignmentRepository.SearchAsync(
                    courseId,
                    normalizedPage,
                    normalizedPageSize,
                    cancellationToken);

            var responses = items
                .Select(ToResponse)
                .ToList();

            return new PagedResult<AssignmentResponse>(
                responses,
                totalCount,
                normalizedPage,
                normalizedPageSize);
        }

        public async Task<AssignmentResponse> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var assignment =
                await RequireAssignmentAsync(id, cancellationToken);

            return ToResponse(assignment);
        }

        public async Task<AssignmentResponse> CreateAsync(
     CreateAssignmentRequest request,
     CancellationToken cancellationToken = default)
        {
            if (request.CourseId == Guid.Empty)
            {
                throw new ValidationException("CourseId is required.");
            }

            var course = await _courseRepository.GetByIdAsync(
                request.CourseId,
                cancellationToken)
                ?? throw new NotFoundException("Course", request.CourseId);

            if (!course.IsActive)
            {
                throw new ValidationException(
                    $"Course '{course.Name}' is not active — reactivate it before adding assignments.");
            }

            var assignment = Assignment.Create(
                request.CourseId,
                request.CreatedByTeacherId,
                request.Title,
                request.Description,
                request.MaxMarks,
                request.DueDate);

            await _assignmentRepository.AddAsync(
                assignment,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(assignment);
        }

        public async Task<AssignmentResponse> UpdateAsync(
            Guid id,
            UpdateAssignmentRequest request,
            CancellationToken cancellationToken = default)
        {
            var assignment =
                await RequireAssignmentAsync(id, cancellationToken);

            assignment.Update(
                request.Title,
                request.Description,
                request.MaxMarks,
                request.DueDate);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(assignment);
        }

        public async Task<AssignmentResponse> ActivateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var assignment =
                await RequireAssignmentAsync(id, cancellationToken);

            assignment.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(assignment);
        }

        public async Task<AssignmentResponse> DeactivateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var assignment =
                await RequireAssignmentAsync(id, cancellationToken);

            assignment.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(assignment);
        }

        private async Task<Assignment> RequireAssignmentAsync(
            Guid id,
            CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                throw new ValidationException("AssignmentId is required.");
            }

            return await _assignmentRepository.GetByIdAsync(
                       id,
                       cancellationToken)
                   ?? throw new NotFoundException("Assignment", id);
        }

        private static AssignmentResponse ToResponse(
            Assignment assignment)
        {
            return new AssignmentResponse(
                assignment.Id,
                assignment.CourseId,
                assignment.CreatedByTeacherId,
                assignment.Title,
                assignment.Description,
                assignment.MaxMarks,
                assignment.DueDate,
                assignment.IsActive,
                assignment.CreatedAt,
                assignment.UpdatedAt);
        }

        public async Task<IReadOnlyList<RosterEntryResponse>> GetRosterAsync(Guid assignmentId, CancellationToken cancellationToken = default)
        {
            var assignment = await RequireAssignmentAsync(assignmentId, cancellationToken);

            var studentIds = await _enrollmentRepository.GetEnrolledStudentIdsByCourseIdAsync(assignment.CourseId, cancellationToken);
            var submissions = await _submissionRepository.GetByAssignmentAsync(assignmentId, cancellationToken);
            var submissionByStudentId = submissions.ToDictionary(s => s.StudentId);

            var roster = new List<RosterEntryResponse>();

            // N+1 lookups here are acceptable at this scale — same trade-off
            // already made in UserManagementService.SearchAsync.
            foreach (var studentId in studentIds)
            {
                var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
                if (student is null) continue; // defensive — shouldn't happen given the FK

                submissionByStudentId.TryGetValue(studentId, out var submission);

                roster.Add(new RosterEntryResponse(
                    studentId,
                    $"{student.FirstName} {student.LastName}",
                    submission is null ? null : ToSubmissionResponse(submission)));
            }

            return roster;
        }

        public async Task<SubmissionResponse> GradeSubmissionAsync(Guid submissionId, GradeSubmissionRequest request, Guid graderUserId, CancellationToken cancellationToken = default)
        {
            var submission = await _submissionRepository.GetByIdAsync(submissionId, cancellationToken)
                ?? throw new NotFoundException("Submission", submissionId);

            var assignment = await RequireAssignmentAsync(submission.AssignmentId, cancellationToken);

            // Grader might be an Admin with no Teacher profile — that's fine,
            // GradedByTeacherId is nullable exactly for this case (see the
            // Grade() domain method).
            var grader = await _teacherRepository.GetByUserIdAsync(graderUserId, cancellationToken);

            submission.Grade(request.Marks, request.Feedback, assignment.MaxMarks, grader?.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToSubmissionResponse(submission);
        }

        private static SubmissionResponse ToSubmissionResponse(AssignmentSubmission submission) => new(
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
