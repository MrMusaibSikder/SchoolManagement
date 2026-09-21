using CourseHub.Domain.Common;
using CourseHub.Domain.Enums;
using CourseHub.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Domain.Entities
{
    public enum SubmissionType
    {
        Text = 1,
        File = 2
    }
    /// <summary>
    /// Represents a student's submission for an assignment.
    /// A student can have only one submission per assignment.
    /// </summary>
    public class AssignmentSubmission : BaseEntity
    {
        public Guid AssignmentId { get; private set; }

        public Guid StudentId { get; private set; }

       

        public SubmissionType SubmissionType { get; private set; }

        public string? TextContent { get; private set; }

        public string? FilePath { get; private set; }

        public string? OriginalFileName { get; private set; }

        public DateTime SubmittedAt { get; private set; }

        public bool IsLate { get; private set; }

        public int? Marks { get; private set; }

        public string? Feedback { get; private set; }

        public DateTime? GradedAt { get; private set; }

        public Guid? GradedByTeacherId { get; private set; }

        public SubmissionStatus Status { get; private set; } = SubmissionStatus.Submitted;

        // Navigation Properties
        public Assignment Assignment { get; private set; } = null!;

        public Student Student { get; private set; } = null!;

      

        public Teacher? GradedByTeacher { get; private set; }

        private AssignmentSubmission()
        {
        }

        private AssignmentSubmission(
            Guid assignmentId,
            Guid studentId,
           
            SubmissionType submissionType,
            string? textContent,
            string? filePath,
            string? originalFileName,
            DateTime submittedAt,
            bool isLate)
        {
            AssignmentId = assignmentId;
            StudentId = studentId;
           
            SubmissionType = submissionType;
            TextContent = textContent;
            FilePath = filePath;
            OriginalFileName = originalFileName;
            SubmittedAt = submittedAt;
            IsLate = isLate;
            Status = SubmissionStatus.Submitted;
        }

        public static AssignmentSubmission CreateTextSubmission(
            Guid assignmentId,
            Guid studentId,
          
            string textContent,
            DateTime submittedAt,
            bool isLate)
        {
            ValidateIds(assignmentId, studentId);

            if (string.IsNullOrWhiteSpace(textContent))
            {
                throw new ValidationException("TextContent is required.");
            }

            return new AssignmentSubmission(
                assignmentId,
                studentId,
               
                SubmissionType.Text,
                textContent.Trim(),
                null,
                null,
                submittedAt.ToUniversalTime(),
                isLate);
        }

        public static AssignmentSubmission CreateFileSubmission(
            Guid assignmentId,
            Guid studentId,
            string filePath,
            string originalFileName,
            DateTime submittedAt,
            bool isLate)
        {
            ValidateIds(assignmentId, studentId);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ValidationException("FilePath is required.");
            }

            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                throw new ValidationException("OriginalFileName is required.");
            }

            return new AssignmentSubmission(
                assignmentId,
                studentId,
               
                SubmissionType.File,
                null,
                filePath,
                originalFileName.Trim(),
                submittedAt.ToUniversalTime(),
                isLate);
        }

        public void ReplaceTextSubmission(
            string textContent,
            DateTime submittedAt,
            bool isLate)
        {
            if (IsGraded())
            {
                throw new ValidationException(
                    "A graded submission cannot be replaced.");
            }

            if (string.IsNullOrWhiteSpace(textContent))
            {
                throw new ValidationException("TextContent is required.");
            }

            SubmissionType = SubmissionType.Text;
            TextContent = textContent.Trim();
            FilePath = null;
            OriginalFileName = null;
            SubmittedAt = submittedAt.ToUniversalTime();
            IsLate = isLate;

            MarkAsUpdated();
        }

        public void ReplaceFileSubmission(
            string filePath,
            string originalFileName,
            DateTime submittedAt,
            bool isLate)
        {
            if (IsGraded())
            {
                throw new ValidationException(
                    "A graded submission cannot be replaced.");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ValidationException("FilePath is required.");
            }

            if (string.IsNullOrWhiteSpace(originalFileName))
            {
                throw new ValidationException("OriginalFileName is required.");
            }

            SubmissionType = SubmissionType.File;
            TextContent = null;
            FilePath = filePath;
            OriginalFileName = originalFileName.Trim();
            SubmittedAt = submittedAt.ToUniversalTime();
            IsLate = isLate;

            MarkAsUpdated();
        }

        public void Grade(
            int marks,
            string? feedback,
            int maxMarks,
            Guid? gradedByTeacherId)
        {
           

            if (marks < 0 || marks > maxMarks)
            {
                throw new ValidationException(
                    $"Marks must be between 0 and {maxMarks}.");
            }

            Marks = marks;
            Feedback = feedback?.Trim();
            GradedAt = DateTime.UtcNow;
            GradedByTeacherId = gradedByTeacherId;
            Status = SubmissionStatus.Graded;

            MarkAsUpdated();
        }

        private bool IsGraded()
        {
            return Status == SubmissionStatus.Graded;
        }

        private static void ValidateIds(
            Guid assignmentId,
            Guid studentId
          )
        {
            if (assignmentId == Guid.Empty)
            {
                throw new ValidationException("AssignmentId is required.");
            }

            if (studentId == Guid.Empty)
            {
                throw new ValidationException("StudentId is required.");
            }

           
        }
    }
}
