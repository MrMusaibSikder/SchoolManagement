using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class FinalResultConfiguration : IEntityTypeConfiguration<FinalResult>
{
    public void Configure(EntityTypeBuilder<FinalResult> builder)
    {
        builder.ToTable("FinalResults");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FinalMarks).HasColumnType("decimal(18,2)");
        builder.Property(x => x.FinalGPA).HasColumnType("decimal(5,2)");
        builder.Property(x => x.FinalGrade).IsRequired().HasMaxLength(10);
        builder.Property(x => x.TeacherRemarks).HasMaxLength(500);
        builder.Property(x => x.PrincipalRemarks).HasMaxLength(500);
        builder.Property(x => x.IsPassed).IsRequired();
        builder.Property(x => x.PromotionStatus).IsRequired();
        builder.Property(x => x.IsPublished).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.Student)
               .WithMany()
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AcademicYear)
               .WithMany()
               .HasForeignKey(x => x.AcademicYearId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ExamWeightSetup)
               .WithMany()
               .HasForeignKey(x => x.ExamWeightSetupId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Details)
               .WithOne(x => x.FinalResult)
               .HasForeignKey(x => x.FinalResultId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.StudentId);
        builder.HasIndex(x => x.AcademicYearId);
        builder.HasIndex(x => x.IsPublished);

        // One final result per student per academic year.
        builder.HasIndex(x => new { x.StudentId, x.AcademicYearId }).IsUnique();

        builder.ToTable(t => t.HasCheckConstraint("CK_FinalResult_GPA", "[FinalGPA] >= 0 AND [FinalGPA] <= 5"));
    }
}
