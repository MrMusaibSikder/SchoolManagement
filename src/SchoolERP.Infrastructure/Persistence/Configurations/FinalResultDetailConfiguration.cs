using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class FinalResultDetailConfiguration : IEntityTypeConfiguration<FinalResultDetail>
{
    public void Configure(EntityTypeBuilder<FinalResultDetail> builder)
    {
        builder.ToTable("FinalResultDetails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FinalMarks).HasColumnType("decimal(18,2)");
        builder.Property(x => x.FinalGradeLabel).IsRequired().HasMaxLength(10);
        builder.Property(x => x.FinalGradePoint).HasColumnType("decimal(5,2)");
        builder.Property(x => x.IsOptional).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.FinalResult)
               .WithMany(x => x.Details)
               .HasForeignKey(x => x.FinalResultId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Subject)
               .WithMany()
               .HasForeignKey(x => x.SubjectId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.FinalResultId);

        // One subject entry per final result.
        builder.HasIndex(x => new { x.FinalResultId, x.SubjectId }).IsUnique();
    }
}
