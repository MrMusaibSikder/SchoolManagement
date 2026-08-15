using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.ConfigureBaseEntity();

        builder.Property(u => u.InstitutionId)
            .IsRequired();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        // Tenant-scoped uniqueness: the same email may exist under
        // different institutions, but not twice within the same one.
        builder.HasIndex(u => new { u.InstitutionId, u.Email })
            .IsUnique();

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.LastLoginAt)
            .HasColumnType("timestamptz");

        // Restrict, not Cascade: deleting an Institution must never
        // silently wipe out its Users.
        builder.HasOne<Institution>()
            .WithMany()
            .HasForeignKey(u => u.InstitutionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
