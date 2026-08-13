using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SchoolConfiguration : IEntityTypeConfiguration<School>
{
    public void Configure(EntityTypeBuilder<School> builder)
    {
        builder.ToTable("Schools");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.EIIN)
               .HasMaxLength(20);

        builder.HasIndex(x => x.EIIN)
               .IsUnique()
               .HasFilter("[EIIN] IS NOT NULL");

        builder.Property(x => x.Address)
               .HasMaxLength(500);

        builder.Property(x => x.Phone)
               .HasMaxLength(20);

        builder.Property(x => x.Email)
               .HasMaxLength(150);

        builder.HasIndex(x => x.Email)
               .IsUnique()
               .HasFilter("[Email] IS NOT NULL");

        builder.Property(x => x.Logo)
               .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);
    }
}