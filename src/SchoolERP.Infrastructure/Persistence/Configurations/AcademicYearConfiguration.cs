using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.ToTable("AcademicYears");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(x => x.Name)
               .IsUnique();

        builder.Property(x => x.StartDate)
               .IsRequired();

        builder.Property(x => x.EndDate)
               .IsRequired();

        builder.Property(x => x.IsCurrent)
               .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);
        // ADD: Only one academic year can be "current" at a time
        builder.HasIndex(x => x.IsCurrent)
               .IsUnique()
               .HasFilter("[IsCurrent] = 1")
               .HasDatabaseName("IX_AcademicYear_OnlyOneCurrent");

        // Check Constraint: EndDate must be greater than StartDate
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_AcademicYear_DateRange",
                "[EndDate] > [StartDate]");
        });
    }
}