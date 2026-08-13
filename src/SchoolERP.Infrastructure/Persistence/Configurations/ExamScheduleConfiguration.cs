using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ExamScheduleConfiguration : IEntityTypeConfiguration<ExamSchedule>
{
    public void Configure(EntityTypeBuilder<ExamSchedule> builder)
    {
        builder.ToTable("ExamSchedules");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.ExamDate)
               .IsRequired();

        builder.Property(x => x.FullMarks)
               .IsRequired();

        builder.Property(x => x.PassMarks)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationships

        // Exam -> ExamSchedules
        builder.HasOne(x => x.Exam)
               .WithMany(x => x.ExamSchedules)
               .HasForeignKey(x => x.ExamId)
               .OnDelete(DeleteBehavior.Cascade);

        // Class -> ExamSchedules
        builder.HasOne(x => x.SchoolClass)
               .WithMany()
               .HasForeignKey(x => x.ClassId)
               .OnDelete(DeleteBehavior.Restrict);

        // Subject -> ExamSchedules
        builder.HasOne(x => x.Subject)
               .WithMany()
               .HasForeignKey(x => x.SubjectId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ExamId);
        builder.HasIndex(x => x.ClassId);
        builder.HasIndex(x => x.SubjectId);

        // Unique Constraint: same subject can't be scheduled twice in same exam & class
        builder.HasIndex(x => new { x.ExamId, x.ClassId, x.SubjectId })
               .IsUnique();
    }
}