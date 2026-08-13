using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
{
    public void Configure(EntityTypeBuilder<ExamResult> builder)
    {
        builder.ToTable("ExamResults");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalMarks).HasColumnType("decimal(18,2)");
        builder.Property(x => x.TotalFullMarks).HasColumnType("decimal(18,2)");
        builder.Property(x => x.Percentage).HasColumnType("decimal(5,2)");
        builder.Property(x => x.GPA).HasColumnType("decimal(5,2)");
        builder.Property(x => x.Grade).IsRequired().HasMaxLength(10);
        builder.Property(x => x.TeacherRemarks).HasMaxLength(500);
        builder.Property(x => x.GuardianRemarks).HasMaxLength(500);
        builder.Property(x => x.IsPassed).IsRequired();
        builder.Property(x => x.IsPublished).HasDefaultValue(false);
        builder.Property(x => x.PublishedAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.Student)
               .WithMany()
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Exam)
               .WithMany()
               .HasForeignKey(x => x.ExamId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.ExamId);
        builder.HasIndex(x => x.IsPublished);

        // One aggregate result per student per exam.
        builder.HasIndex(x => new { x.StudentId, x.ExamId }).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_ExamResult_Percentage", "[Percentage] >= 0 AND [Percentage] <= 100");
            t.HasCheckConstraint("CK_ExamResult_GPA", "[GPA] >= 0 AND [GPA] <= 5");
        });
    }
}
