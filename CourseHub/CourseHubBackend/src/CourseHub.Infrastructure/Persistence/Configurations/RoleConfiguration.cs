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

        // Nullable: null = platform-wide system role, non-null = institution role.
        builder.Property(r => r.InstitutionId);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasColumnType("text");

        builder.Property(r => r.IsActive)
            .IsRequired();

        builder.Property(r => r.IsSystemRole)
            .IsRequired();

        // A plain composite unique index on (InstitutionId, Name) would NOT
        // stop two system roles (InstitutionId = NULL) from sharing a name,
        // because PostgreSQL treats every NULL as distinct in a unique index.
        // Two partial/filtered indexes give the intended semantics instead:

        // Institution-scoped roles: name unique within that institution.
        builder.HasIndex(r => new { r.InstitutionId, r.Name })
            .IsUnique()
            .HasFilter("\"InstitutionId\" IS NOT NULL")
            .HasDatabaseName("IX_Roles_InstitutionId_Name_InstitutionRoles");

        // System roles: name unique platform-wide among InstitutionId IS NULL rows.
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasFilter("\"InstitutionId\" IS NULL")
            .HasDatabaseName("IX_Roles_Name_SystemRoles");

        // Optional relationship: system roles have no Institution.
        builder.HasOne<Institution>()
            .WithMany()
            .HasForeignKey(r => r.InstitutionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
