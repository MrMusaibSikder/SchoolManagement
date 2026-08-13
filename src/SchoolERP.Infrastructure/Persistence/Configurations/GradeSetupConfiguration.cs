using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class GradeSetupConfiguration : IEntityTypeConfiguration<GradeSetup>
{
    public void Configure(EntityTypeBuilder<GradeSetup> builder)
    {
        builder.ToTable("GradeSetups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.GradeName).IsRequired().HasMaxLength(10);
        builder.Property(x => x.GradePoint).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(x => x.MinMarks).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MaxMarks).HasColumnType("decimal(18,2)");
        builder.Property(x => x.MinPercentage).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(x => x.MaxPercentage).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(x => x.IsFail).HasDefaultValue(false);
        builder.Property(x => x.DisplayOrder).IsRequired();
        builder.Property(x => x.IsActive).HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.AcademicYear)
               .WithMany()
               .HasForeignKey(x => x.AcademicYearId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.AcademicYearId);
        builder.HasIndex(x => new { x.AcademicYearId, x.GradeName }).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_GradeSetup_Percentage", "[MinPercentage] >= 0 AND [MaxPercentage] <= 100 AND [MinPercentage] <= [MaxPercentage]");
            t.HasCheckConstraint("CK_GradeSetup_GradePoint", "[GradePoint] >= 0 AND [GradePoint] <= 5");
        });
    }
}
