using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations
{
    public class FeeCategoryConfiguration : IEntityTypeConfiguration<FeeCategory>
    {
        public void Configure(EntityTypeBuilder<FeeCategory> builder)
        {
            builder.ToTable("FeeCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Description)
                   .HasMaxLength(300);

            builder.Property(x => x.DisplayOrder)
                   .HasDefaultValue(0);

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            builder.HasIndex(x => x.Name)
                   .IsUnique()
                   .HasDatabaseName("IX_FeeCategory_Name");

            builder.HasMany(x => x.FeeTypes)
                   .WithOne(x => x.FeeCategory)
                   .HasForeignKey(x => x.FeeCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
