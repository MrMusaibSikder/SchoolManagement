using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.ConfigureBaseEntity();

        builder.Property(s => s.InstitutionId)
            .IsRequired();

        builder.Property(s => s.UserId)
            .IsRequired();

        // One student profile per user — same reasoning as Teacher.UserId
        // above; flagged as a judgment call, not explicitly in the spec.
        builder.HasIndex(s => s.UserId)
            .IsUnique();

        builder.Property(s => s.StudentId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => new { s.InstitutionId, s.StudentId })
            .IsUnique();

        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(s => s.DateOfBirth)
            .HasColumnType("date");

        builder.Property(s => s.Phone)
            .HasMaxLength(30);

        builder.Property(s => s.Email)
            .HasMaxLength(255);

        builder.Property(s => s.Address)
            .HasMaxLength(300);

        builder.Property(s => s.GuardianName)
            .HasMaxLength(150);

        builder.Property(s => s.GuardianPhone)
            .HasMaxLength(30);

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.IsProfilePublic)
            .IsRequired();

        builder.HasOne<Institution>()
            .WithMany()
            .HasForeignKey(s => s.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
