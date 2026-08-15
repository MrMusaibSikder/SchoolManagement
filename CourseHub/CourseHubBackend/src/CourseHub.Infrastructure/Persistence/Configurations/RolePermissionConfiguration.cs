using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");

        builder.ConfigureBaseEntity();

        builder.Property(rp => rp.RoleId)
            .IsRequired();

        builder.Property(rp => rp.PermissionId)
            .IsRequired();

        // A role should not have the same permission assigned twice.
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique();

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Permissions are global/reusable — never let a role deletion
        // cascade into the shared permission catalog.
        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
