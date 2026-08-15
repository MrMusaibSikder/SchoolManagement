using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.ConfigureBaseEntity();

        builder.Property(e => e.InstitutionId)
            .IsRequired();

        builder.Property(e => e.StudentId)
            .IsRequired();

        builder.Property(e => e.BatchId)
            .IsRequired();

        // A student should not be enrolled in the same batch more than once.
        builder.HasIndex(e => new { e.StudentId, e.BatchId })
            .IsUnique();

        builder.Property(e => e.EnrollmentDate)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.HasOne<Institution>()
            .WithMany()
            .HasForeignKey(e => e.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict on both: enrollment history must survive a Student or
        // Batch being removed — it is never implicitly cascaded away.
        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Batch>()
            .WithMany()
            .HasForeignKey(e => e.BatchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
