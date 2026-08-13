using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Code)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(x => x.Code)
               .IsUnique();

        builder.Property(x => x.FullMarks)
               .IsRequired();

        builder.Property(x => x.PassMarks)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Validation
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Subject_FullMarks",
                "[FullMarks] > 0");

            t.HasCheckConstraint(
                "CK_Subject_PassMarks",
                "[PassMarks] >= 0 AND [PassMarks] <= [FullMarks]");
        });
    }
}