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
    public class AssignmentSubmissionService : IAssignmentSubmissionService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignmentSubmissionService( 
            IStudentRepository studentRepository,
            IAssignmentRepository assignmentRepository,
            ISubmissionRepository submissionRepository,
            IEnrollmentRepository enrollmentRepository,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _studentRepository = studentRepository;
            _assignmentRepository = assignmentRepository;
            _submissionRepository = submissionRepository;
            _enrollmentRepository = enrollmentRepository;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<SubmissionResponse> SubmitTextAsync(
            Guid assignmentId,
            Guid studentId,
            SubmitTextAssignmentRequest request,
            CancellationToken cancellationToken = default)
        {
            if (assignmentId == Guid.Empty)
                throw new ValidationException("AssignmentId is required.");

            if (studentId == Guid.Empty)
                throw new ValidationException("StudentId is required.");

            var assignment = await _assignmentRepository.GetByIdAsync(
                assignmentId,
                cancellationToken);

            if (assignment is null)
                throw new NotFoundException("Assignment", assignmentId);

            if (!assignment.IsActive)
                throw new ValidationException("This assignment is not active.");

            var isEnrolled =
            await _enrollmentRepository.ExistsForStudentAndCourseAsync(
            studentId,
            assignment.CourseId,
            cancellationToken);

            if (!isEnrolled)
                throw new ValidationException(
                    "Student is not enrolled in this course.");


            var submittedAt = DateTime.UtcNow;
            var isLate = submittedAt > assignment.DueDate;

            var existingSubmission =
                await _submissionRepository.GetByAssignmentAndStudentAsync(
                    assignmentId,
                    studentId,
                    cancellationToken);

            if (existingSubmission is not null)
            {
                existingSubmission.ReplaceTextSubmission(
                    request.TextContent,
                    submittedAt,
                    isLate);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return ToResponse(existingSubmission);
            }



            var submission = AssignmentSubmission.CreateTextSubmission(
                assignmentId,
                studentId,
                request.TextContent,
                submittedAt,
                isLate);

            await _submissionRepository.AddAsync(
                submission,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ToResponse(submission);
        }

        public async Task<SubmissionResponse> GetMySubmissionAsync(
            Guid assignmentId,
            Guid studentId,
            CancellationToken cancellationToken = default)
        {
            if (assignmentId == Guid.Empty)
                throw new ValidationException("AssignmentId is required.");

            if (studentId == Guid.Empty)
                throw new ValidationException("StudentId is required.");

            var submission =
                await _submissionRepository.GetByAssignmentAndStudentAsync(
                    assignmentId,
                    studentId,
                    cancellationToken);

            if (submission is null)
                throw new NotFoundException(
                    "Assignment submission",
                    assignmentId);

            return ToResponse(submission);
        }

        public async Task<SubmissionResponse> GradeAsync(
      Guid submissionId,
      Guid? teacherId,
      GradeSubmissionRequest request,
      CancellationToken cancellationToken = default)
        {
            if (submissionId == Guid.Empty)
                throw new ValidationException("SubmissionId is required.");

            // teacherId == Guid.Empty check বাদ — null মানেই বৈধ (Admin, Teacher profile নেই)

            var submission = await _submissionRepository.GetByIdAsync(submissionId, cancellationToken)
                ?? throw new NotFoundException("Assignment submission", submissionId);

            var assignment = await _assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken)
                ?? throw new NotFoundException("Assignment", submission.AssignmentId);

            submission.Grade(request.Marks, request.Feedback, assignment.MaxMarks, teacherId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ToResponse(submission);
        }



        private static SubmissionResponse ToResponse(
            AssignmentSubmission submission)
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

        public async Task<SubmissionResponse> SubmitFileAsync(
      Guid assignmentId,
      Guid studentId,
      Stream fileStream,
      string fileName,
      string contentType,
      CancellationToken cancellationToken = default)
        {
            if (assignmentId == Guid.Empty)
                throw new ValidationException("AssignmentId is required.");

            if (studentId == Guid.Empty)
                throw new ValidationException("StudentId is required.");

            var assignment = await _assignmentRepository.GetByIdAsync(
                assignmentId,
                cancellationToken)
                ?? throw new NotFoundException("Assignment", assignmentId);

            if (!assignment.IsActive)
                throw new ValidationException("This assignment is not active.");

            var isEnrolled =
                await _enrollmentRepository.ExistsForStudentAndCourseAsync(
                    studentId,
                    assignment.CourseId,
                    cancellationToken);

            if (!isEnrolled)
                throw new ValidationException(
                    "Student is not enrolled in this course.");

            var safeFileName = Path.GetFileName(fileName);

            if (string.IsNullOrWhiteSpace(safeFileName))
                throw new ValidationException("File name is required.");

            var submittedAt = DateTime.UtcNow;
            var isLate = submittedAt > assignment.DueDate;

            // Check existing submission BEFORE saving the new file.
            var existingSubmission =
                await _submissionRepository.GetByAssignmentAndStudentAsync(
                    assignmentId,
                    studentId,
                    cancellationToken);

            // If a submission already exists, replace it.
            if (existingSubmission is not null)
            {
                var storageKey = await _fileStorageService.SaveAsync(
                    fileStream,
                    safeFileName,
                    contentType,
                    cancellationToken);

                try
                {
                    existingSubmission.ReplaceFileSubmission(
                        storageKey,
                        safeFileName,
                        submittedAt,
                        isLate);

                    await _unitOfWork.SaveChangesAsync(
                        cancellationToken);
                }
                catch
                {
                    // Remove newly uploaded file if database update fails
                    // or the submission is already graded.
                    await _fileStorageService.DeleteAsync(
                        storageKey,
                        cancellationToken);

                    throw;
                }

                return ToResponse(existingSubmission);
            }

            // No existing submission — create a new one.
            var newStorageKey = await _fileStorageService.SaveAsync(
                fileStream,
                safeFileName,
                contentType,
                cancellationToken);

            try
            {
                var submission = AssignmentSubmission.CreateFileSubmission(
                    assignmentId,
                    studentId,
                    newStorageKey,
                    safeFileName,
                    submittedAt,
                    isLate);

                await _submissionRepository.AddAsync(
                    submission,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    cancellationToken);

                return ToResponse(submission);
            }
            catch
            {
                // Database save failed, so remove the uploaded file.
                await _fileStorageService.DeleteAsync(
                    newStorageKey,
                    cancellationToken);

                throw;
            }
        }

        public async Task<IReadOnlyList<AssignmentRosterResponse>> GetRosterAsync(
    Guid assignmentId,
    CancellationToken cancellationToken = default)
        {
            if (assignmentId == Guid.Empty)
                throw new ValidationException("AssignmentId is required.");

            var assignment = await _assignmentRepository.GetByIdAsync(
                assignmentId,
                cancellationToken);

            if (assignment is null)
                throw new NotFoundException("Assignment", assignmentId);

            // Get every student enrolled in this course.
            var studentIds =
                await _enrollmentRepository.GetEnrolledStudentIdsByCourseIdAsync(
                    assignment.CourseId,
                    cancellationToken);

            if (studentIds.Count == 0)
                return Array.Empty<AssignmentRosterResponse>();

            // Get all student profiles in one database query.
            var students = await _studentRepository.GetByIdsAsync(
                studentIds,
                cancellationToken);

            // Get all submissions for this assignment in one query.
            var submissions =
                await _submissionRepository.GetByAssignmentAsync(
                    assignmentId,
                    cancellationToken);

            var submissionByStudentId = submissions
                .ToDictionary(s => s.StudentId);

            var result = students
                .Select(student =>
                {
                    submissionByStudentId.TryGetValue(
                        student.Id,
                        out var submission);

                    return new AssignmentRosterResponse(
                        student.Id,
                        $"{student.FirstName} {student.LastName}".Trim(),
                        student.StudentId,
                        submission?.Id,
                        submission?.Status.ToString() ?? "NotSubmitted",
                        submission?.SubmissionType.ToString(),
                        submission?.SubmittedAt,
                        submission?.IsLate ?? false,
                        submission?.Marks,
                        submission?.Feedback,
                        submission?.GradedAt);
                })
                .OrderBy(x => x.StudentName)
                .ToList();

            return result;
        }

        public async Task<FileDownloadResult> DownloadFileAsync(
    Guid submissionId,
    Guid? currentUserId,
    CancellationToken cancellationToken = default)
        {
            if (submissionId == Guid.Empty)
                throw new ValidationException("SubmissionId is required.");

            var submission = await _submissionRepository.GetByIdAsync(
                submissionId,
                cancellationToken);

            if (submission is null)
                throw new NotFoundException(
                    "Assignment submission",
                    submissionId);

            if (string.IsNullOrWhiteSpace(submission.FilePath))
                throw new ValidationException(
                    "This submission does not contain a file.");

            return await _fileStorageService.GetAsync(
                submission.FilePath,
                cancellationToken);
        }
    }
}
