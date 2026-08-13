using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Composite Unique Index
        builder.HasIndex(x => new { x.ClassId, x.Name })
               .IsUnique();

        // Relationship
        builder.HasOne(x => x.SchoolClass)
               .WithMany(x => x.Sections)
               .HasForeignKey(x => x.ClassId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Students)
               .WithOne(x => x.Section)
               .HasForeignKey(x => x.SectionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}