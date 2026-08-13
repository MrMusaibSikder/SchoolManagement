using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Persistence.Configurations
{
    public class FeeStructureItemConfiguration : IEntityTypeConfiguration<FeeStructureItem>
    {
        public void Configure(EntityTypeBuilder<FeeStructureItem> builder)
        {
            builder.ToTable("FeeStructureItems");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.IsOptional)
                   .HasDefaultValue(false);

            builder.Property(x => x.SortOrder)
                   .HasDefaultValue(0);

            // Relationships
            builder.HasOne(x => x.FeeStructure)
                   .WithMany(x => x.FeeStructureItems)
                   .HasForeignKey(x => x.FeeStructureId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.FeeType)
                   .WithMany(x => x.FeeStructureItems)
                   .HasForeignKey(x => x.FeeTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Unique Constraint
            builder.HasIndex(x => new
            {
                x.FeeStructureId,
                x.FeeTypeId
            })
            .IsUnique()
            .HasDatabaseName("IX_FeeStructureItems_Structure_FeeType");

            // Performance Index
            builder.HasIndex(x => x.FeeTypeId)
                   .HasDatabaseName("IX_FeeStructureItems_FeeTypeId");

            // Soft Delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
