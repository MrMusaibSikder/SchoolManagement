using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class NoticeConfiguration : IEntityTypeConfiguration<Notice>
{
    public void Configure(EntityTypeBuilder<Notice> builder)
    {
        builder.ToTable("Notices");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(2000);

        builder.Property(x => x.PublishDate)
               .IsRequired();

        builder.Property(x => x.ExpiryDate);

        builder.Property(x => x.Priority)
               .IsRequired()
              ;

        builder.Property(x => x.Audience)
               .IsRequired()
              ;

        builder.Property(x => x.IsPublished)
               .HasDefaultValue(false);

        builder.Property(x => x.IsArchived)
               .HasDefaultValue(false);

        builder.Property(x => x.SendSms)
               .HasDefaultValue(false);

        builder.Property(x => x.SendEmail)
               .HasDefaultValue(false);

        builder.Property(x => x.AttachmentPath)
               .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(x => x.PublishDate);

        builder.HasIndex(x => x.ExpiryDate);

        builder.HasIndex(x => x.Priority);

        builder.HasIndex(x => x.Audience);

        builder.HasIndex(x => new { x.IsPublished, x.IsArchived });
    }
}