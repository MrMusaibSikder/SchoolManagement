using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ExamWeightItemConfiguration : IEntityTypeConfiguration<ExamWeightItem>
{
    public void Configure(EntityTypeBuilder<ExamWeightItem> builder)
    {
        builder.ToTable("ExamWeightItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WeightPercentage)
               .HasColumnType("decimal(5,2)")
               .IsRequired();

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);

        builder.HasOne(x => x.ExamWeightSetup)
               .WithMany(x => x.Items)
               .HasForeignKey(x => x.ExamWeightSetupId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Exam)
               .WithMany()
               .HasForeignKey(x => x.ExamId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ExamWeightSetupId);

        // One weight entry per exam within a given setup.
        builder.HasIndex(x => new { x.ExamWeightSetupId, x.ExamId }).IsUnique();

        builder.ToTable(t => t.HasCheckConstraint(
            "CK_ExamWeightItem_Percentage",
            "[WeightPercentage] > 0 AND [WeightPercentage] <= 100"));
    }
}
