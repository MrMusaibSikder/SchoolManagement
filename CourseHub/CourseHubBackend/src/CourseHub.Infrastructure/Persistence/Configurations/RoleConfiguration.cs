using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.ConfigureBaseEntity();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Single-institute: role names are globally unique (no more
        // institution-scoped vs system-role split).
        builder.HasIndex(r => r.Name)
            .IsUnique();

        builder.Property(r => r.Description)
            .HasColumnType("text");

        builder.Property(r => r.IsActive)
            .IsRequired();

        builder.Property(r => r.IsSystemRole)
            .IsRequired();
    }
}
