using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SmsLogConfiguration : IEntityTypeConfiguration<SmsLog>
{
    public void Configure(EntityTypeBuilder<SmsLog> builder)
    {
        builder.ToTable("SmsLogs");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.RecipientNumber)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.Message)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.Status)
               .IsRequired();

        builder.Property(x => x.ProviderResponse)
               .HasMaxLength(1000);

        builder.Property(x => x.Provider)
               .HasMaxLength(50);

        builder.Property(x => x.SentAt);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationships
        builder.HasOne(x => x.Student)
               .WithMany()
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => x.RecipientNumber);

        builder.HasIndex(x => x.Status);

        builder.HasIndex(x => x.SentAt);

        builder.HasIndex(x => x.Provider);

        builder.HasIndex(x => new { x.StudentId, x.Status });
    }
}