using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder.ToTable("Results");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.MarksObtained)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.GraceMarks)
               .HasColumnType("decimal(18,2)")
               .HasDefaultValue(0m);

        builder.Property(x => x.Grade)
               .HasMaxLength(10);

        builder.Property(x => x.GPA)
               .HasColumnType("decimal(5,2)");

        builder.Property(x => x.Percentage)
               .HasColumnType("decimal(5,2)");

        builder.Property(x => x.IsPassed)
               .IsRequired();

        builder.Property(x => x.AttendanceStatus)
               .IsRequired()
               ;

        builder.Property(x => x.EntryStatus)
               .IsRequired()
              ;

        builder.Property(x => x.Remarks)
               .HasMaxLength(500);

        builder.Property(x => x.IsLocked)
               .HasDefaultValue(false);

        builder.Property(x => x.LockedAt);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationships

        // Student -> Results
        builder.HasOne(x => x.Student)
               .WithMany(x => x.Results)
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        // ExamSchedule -> Results
        builder.HasOne(x => x.ExamSchedule)
               .WithMany(x => x.Results)
               .HasForeignKey(x => x.ExamScheduleId)
               .OnDelete(DeleteBehavior.Cascade);

        // Teacher (entered by) -> Results
        builder.HasOne(x => x.EnteredByTeacher)
               .WithMany()
               .HasForeignKey(x => x.EnteredByTeacherId)
               .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.ExamScheduleId);
        builder.HasIndex(x => x.EnteredByTeacherId);
        builder.HasIndex(x => x.EntryStatus);
        builder.HasIndex(x => x.IsLocked);

        // Unique Constraint: one student has only one result per exam schedule
        builder.HasIndex(x => new { x.StudentId, x.ExamScheduleId })
               .IsUnique();

        // Validation Rules (DB level)
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Result_Marks",
                "[MarksObtained] >= 0");

            t.HasCheckConstraint(
                "CK_Result_GraceMarks",
                "[GraceMarks] >= 0");

            t.HasCheckConstraint(
                "CK_Result_Percentage",
                "[Percentage] >= 0 AND [Percentage] <= 100");

            t.HasCheckConstraint(
                "CK_Result_GPA",
                "[GPA] >= 0 AND [GPA] <= 5");
        });
    }
}