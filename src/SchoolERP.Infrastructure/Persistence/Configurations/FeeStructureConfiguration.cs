using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class FeeStructureConfiguration : IEntityTypeConfiguration<FeeStructure>
{
    public void Configure(EntityTypeBuilder<FeeStructure> builder)
    {
        builder.ToTable("FeeStructures");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.EffectiveFrom)
               .IsRequired();

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        // Relationships
        builder.HasOne(x => x.AcademicYear)
               .WithMany(x => x.FeeStructures)
               .HasForeignKey(x => x.AcademicYearId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SchoolClass)
               .WithMany(x => x.FeeStructures)
               .HasForeignKey(x => x.SchoolClassId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Section)
               .WithMany(x => x.FeeStructures)
               .HasForeignKey(x => x.SectionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.FeeStructureItems)
               .WithOne(x => x.FeeStructure)
               .HasForeignKey(x => x.FeeStructureId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Invoices)
               .WithOne(x => x.FeeStructure)
               .HasForeignKey(x => x.FeeStructureId)
               .OnDelete(DeleteBehavior.Restrict);

        //  Filtered Unique Indexes — Critical Fix

        // Section-specific fee structure
        builder.HasIndex(x => new { x.SchoolClassId, x.SectionId, x.AcademicYearId })
               .IsUnique()
               .HasFilter("[SectionId] IS NOT NULL")
               .HasDatabaseName("IX_FeeStructures_Class_Section_AcademicYear");

        // Class-wide fee structure (no section)
        builder.HasIndex(x => new { x.SchoolClassId, x.AcademicYearId })
               .IsUnique()
               .HasFilter("[SectionId] IS NULL")
               .HasDatabaseName("IX_FeeStructures_Class_AcademicYear");

        // Soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
