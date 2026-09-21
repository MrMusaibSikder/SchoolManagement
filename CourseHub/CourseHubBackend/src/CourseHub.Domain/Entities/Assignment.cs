using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Domain.Entities
{

    /// <summary>
    /// Assignment represents an academic task created for a Course.
    /// Every active batch of the course and its enrolled students can access
    /// the assignment.
    /// </summary>
    public class Assignment : BaseEntity
    {
        public Guid CourseId { get; private set; }

       

        public Guid? CreatedByTeacherId { get; private set; }

        public string Title { get; private set; } = null!;

        public string? Description { get; private set; }

        public int MaxMarks { get; private set; }

        public DateTime DueDate { get; private set; }

        public bool IsActive { get; private set; }

        // Navigation Properties
        public Course Course { get; private set; } = null!;
      

        public Teacher? CreatedByTeacher { get; private set; }

        public ICollection<AssignmentSubmission> Submissions { get; private set; }
            = new List<AssignmentSubmission>();

        private Assignment()
        {
        }

        private Assignment(
            Guid courseId,
            Guid? createdByTeacherId,
            string title,
            string? description,
            int maxMarks,
            DateTime dueDate)
        {
            CourseId = courseId;
            CreatedByTeacherId = createdByTeacherId;
            Title = title;
            Description = description;
            MaxMarks = maxMarks;
            DueDate = dueDate;
            IsActive = true;
        }

        public static Assignment Create(
            Guid courseId,
            Guid? createdByTeacherId,
            string title,
            string? description,
            int maxMarks,
            DateTime dueDate)
        {
            if (courseId == Guid.Empty)
            {
                throw new ValidationException("CourseId is required.");
            }
           
            if (createdByTeacherId.HasValue &&
                createdByTeacherId.Value == Guid.Empty)
            {
                throw new ValidationException("CreatedByTeacherId cannot be empty.");
            }

            var validatedTitle = ValidateRequired(title, "Title");
            ValidateMaxMarks(maxMarks);

            return new Assignment(
                courseId,
                createdByTeacherId,
                validatedTitle,
                description?.Trim(),
                maxMarks,
                dueDate.ToUniversalTime());
        }

        public void Update(
            string title,
            string? description,
            int maxMarks,
            DateTime dueDate)
        {
            Title = ValidateRequired(title, "Title");
            Description = description?.Trim();
            ValidateMaxMarks(maxMarks);

            MaxMarks = maxMarks;
            DueDate = dueDate.ToUniversalTime();

            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        private static string ValidateRequired(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ValidationException($"{fieldName} is required.");
            }

            return value.Trim();
        }

        private static void ValidateMaxMarks(int maxMarks)
        {
            if (maxMarks <= 0)
            {
                throw new ValidationException("MaxMarks must be greater than zero.");
            }
        }

    }
   
}