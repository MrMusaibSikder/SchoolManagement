using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");

        builder.ConfigureBaseEntity();

        builder.Property(ur => ur.UserId)
            .IsRequired();

        builder.Property(ur => ur.RoleId)
            .IsRequired();

        // A user should not have the same role assigned twice.
        builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
            .IsUnique();

        // Restrict: removing a Role/User must go through an explicit
        // application-level unassignment, not an implicit cascade.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
