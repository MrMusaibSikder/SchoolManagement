using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ExamWeightSetupConfiguration : IEntityTypeConfiguration<ExamWeightSetup>
{
    public void Configure(EntityTypeBuilder<ExamWeightSetup> builder)
    {
        builder.ToTable("ExamWeightSetups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.IsActive).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.AcademicYear)
               .WithMany()
               .HasForeignKey(x => x.AcademicYearId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Items)
               .WithOne(x => x.ExamWeightSetup)
               .HasForeignKey(x => x.ExamWeightSetupId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.AcademicYearId);
        builder.HasIndex(x => new { x.AcademicYearId, x.IsActive });
    }
}
