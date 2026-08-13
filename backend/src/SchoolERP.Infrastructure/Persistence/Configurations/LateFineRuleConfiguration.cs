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
    public class LateFineRuleConfiguration : IEntityTypeConfiguration<LateFineRule>
    {
        public void Configure(EntityTypeBuilder<LateFineRule> builder)
        {
            builder.ToTable("LateFineRules");

            builder.HasKey(x => x.Id);


            // Properties

            builder.Property(x => x.Type)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();


            builder.Property(x => x.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();


            builder.Property(x => x.GracePeriodDays)
                   .HasDefaultValue(0);


            builder.Property(x => x.MaxFineAmount)
                   .HasPrecision(18, 2);


            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);



            // Relationships

            builder.HasOne(x => x.AcademicYear)
                   .WithMany(x => x.LateFineRules)
                   .HasForeignKey(x => x.AcademicYearId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.FeeType)
                   .WithMany(x => x.LateFineRules)
                   .HasForeignKey(x => x.FeeTypeId)
                   .OnDelete(DeleteBehavior.Restrict);


            //  Unique Indexes — Filtered (Critical Fix)

            // Per fee-type rule: one per AcademicYear + FeeType
            builder.HasIndex(x => new { x.AcademicYearId, x.FeeTypeId })
                   .IsUnique()
                   .HasFilter("[FeeTypeId] IS NOT NULL")
                   .HasDatabaseName("IX_LateFineRule_Year_FeeType");

            // Global rule: one per AcademicYear (FeeTypeId = NULL)
            builder.HasIndex(x => x.AcademicYearId)
                   .IsUnique()
                   .HasFilter("[FeeTypeId] IS NULL")
                   .HasDatabaseName("IX_LateFineRule_Year_Global");

            // Soft delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
