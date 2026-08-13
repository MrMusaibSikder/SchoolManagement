using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exams");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Status)
               .IsRequired()
               ;

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Foreign Keys
        builder.HasOne(x => x.ExamType)
               .WithMany(x => x.Exams)
               .HasForeignKey(x => x.ExamTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AcademicYear)
               .WithMany()
               .HasForeignKey(x => x.AcademicYearId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationships
        builder.HasMany(x => x.ExamSchedules)
               .WithOne(x => x.Exam)
               .HasForeignKey(x => x.ExamId)
               .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint: same exam name cannot repeat for the same exam type
        // within the same academic year (AcademicYear + ExamType + Name).
        builder.HasIndex(x => new { x.AcademicYearId, x.ExamTypeId, x.Name })
               .IsUnique();

        builder.HasIndex(x => x.Status);
    }
}