using CourseHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

/// <summary>
/// Applies the common BaseEntity mapping (Id, CreatedAt, UpdatedAt) shared
/// by every entity configuration, so it isn't repeated in each file.
/// </summary>
internal static class BaseEntityConfigurationExtensions
{
    public static void ConfigureBaseEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.HasKey(e => e.Id);

        // Domain (BaseEntity) generates the Guid itself in its constructor,
        // so EF must never generate/override it.
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamptz");
    }
}
