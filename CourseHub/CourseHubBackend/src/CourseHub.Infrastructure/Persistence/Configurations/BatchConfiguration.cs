using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("Batches");

        builder.ConfigureBaseEntity();

        builder.Property(b => b.CourseId)
            .IsRequired();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(b => b.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(b => b.Code)
            .IsUnique();

        builder.Property(b => b.StartDate)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(b => b.EndDate)
            .HasColumnType("timestamptz");

        builder.Property(b => b.Capacity);

        builder.Property(b => b.IsActive)
            .IsRequired();

        // Restrict, not Cascade: deleting a Course must never silently
        // wipe out the batches (and their enrollment history) run under it.
        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(b => b.CourseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
