using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        builder.ToTable("SchoolClasses");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.DisplayOrder)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Unique Constraints
        builder.HasIndex(x => x.Name)
               .IsUnique();

        builder.HasIndex(x => x.DisplayOrder)
               .IsUnique();

        // Relationship
        builder.HasMany(x => x.Sections)
               .WithOne(x => x.SchoolClass)
               .HasForeignKey(x => x.ClassId)
               .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(x => x.DisplayOrder);
    }
}
