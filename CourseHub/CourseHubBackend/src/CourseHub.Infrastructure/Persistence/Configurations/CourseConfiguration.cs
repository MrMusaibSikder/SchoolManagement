using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.ConfigureBaseEntity();

        builder.Property(c => c.InstitutionId)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(c => new { c.InstitutionId, c.Code })
            .IsUnique();

        builder.Property(c => c.Description)
            .HasColumnType("text");

        builder.Property(c => c.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(c => c.DurationInMonths)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired();

        builder.Property(c => c.IsPublic)
            .IsRequired();

        builder.HasOne<Institution>()
            .WithMany()
            .HasForeignKey(c => c.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
