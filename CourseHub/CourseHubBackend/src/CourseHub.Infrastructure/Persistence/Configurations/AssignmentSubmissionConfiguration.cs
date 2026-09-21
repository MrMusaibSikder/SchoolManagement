using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Infrastructure.Persistence.Configurations
{
    public class AssignmentSubmissionConfiguration
    : IEntityTypeConfiguration<AssignmentSubmission>
    {
        public void Configure(EntityTypeBuilder<AssignmentSubmission> builder)
        {
            builder.ToTable("AssignmentSubmissions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AssignmentId)
                .IsRequired();

            builder.Property(x => x.StudentId)
                .IsRequired();

           

            builder.Property(x => x.SubmissionType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(x => x.TextContent)
                .HasMaxLength(10000);

            builder.Property(x => x.FilePath)
                .HasMaxLength(500);

            builder.Property(x => x.OriginalFileName)
                .HasMaxLength(255);

            builder.Property(x => x.SubmittedAt)
                .IsRequired();

            builder.Property(x => x.IsLate)
                .IsRequired();

            builder.Property(x => x.Marks)
                .IsRequired(false);

            builder.Property(x => x.Feedback)
                .HasMaxLength(2000);

            builder.Property(x => x.GradedAt)
                .IsRequired(false);

            builder.Property(x => x.GradedByTeacherId)
                .IsRequired(false);

            builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

            // Assignment -> Submission
            builder.HasOne(x => x.Assignment)
                .WithMany(x => x.Submissions)
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student -> Submission
            builder.HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

           

            // Graded By Teacher -> Submission
            builder.HasOne(x => x.GradedByTeacher)
                .WithMany()
                .HasForeignKey(x => x.GradedByTeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            // One student can submit only once for an assignment
            builder.HasIndex(x => new
            {
                x.AssignmentId,
                x.StudentId
            })
            .IsUnique();
        }
    }
}
