using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class FeeTypeConfiguration : IEntityTypeConfiguration<FeeType>
{
    public void Configure(EntityTypeBuilder<FeeType> builder)
    {
        builder.ToTable("FeeTypes");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.Description)
               .HasMaxLength(300);

        builder.Property(x => x.Frequency)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(20);

        builder.Property(x => x.IsMandatory)
               .HasDefaultValue(true);

        builder.Property(x => x.IsRefundable)
               .HasDefaultValue(false);

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        builder.Property(x => x.DefaultDueDayOfMonth)
               .IsRequired(false);

        builder.Property(x => x.DefaultGracePeriodDays)
               .HasDefaultValue(5);

        // Relationships
        builder.HasOne(x => x.FeeCategory)
               .WithMany(x => x.FeeTypes)
               .HasForeignKey(x => x.FeeCategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        // Unique Indexes
        builder.HasIndex(x => x.Code)
               .IsUnique()
               .HasDatabaseName("IX_FeeTypes_Code");

        builder.HasIndex(x => x.Name)
               .IsUnique()
               .HasDatabaseName("IX_FeeTypes_Name");

        // Performance Index
        builder.HasIndex(x => x.FeeCategoryId)
               .HasDatabaseName("IX_FeeTypes_FeeCategoryId");

        // Global Query Filter (Soft Delete)
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}