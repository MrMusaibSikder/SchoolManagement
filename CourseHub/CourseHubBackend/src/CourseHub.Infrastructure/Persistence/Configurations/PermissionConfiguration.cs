using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.ConfigureBaseEntity();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(p => p.Name)
            .IsUnique();

        builder.Property(p => p.Description)
            .HasColumnType("text");

        builder.Property(p => p.Resource)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Action)
            .IsRequired()
            .HasMaxLength(100);

        // Prevent defining the same capability twice under different names.
        builder.HasIndex(p => new { p.Resource, p.Action })
            .IsUnique();
    }
}
