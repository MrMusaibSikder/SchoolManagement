using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teachers");

        builder.ConfigureBaseEntity();

        builder.Property(t => t.UserId)
            .IsRequired();

        // One teaching profile per user.
        builder.HasIndex(t => t.UserId)
            .IsUnique();

        builder.Property(t => t.EmployeeId)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(t => t.EmployeeId)
            .IsUnique();

        builder.Property(t => t.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(t => t.Phone)
            .HasMaxLength(30);

        builder.Property(t => t.Email)
            .HasMaxLength(255);

        builder.Property(t => t.Bio)
            .HasColumnType("text");

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.IsProfilePublic)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
