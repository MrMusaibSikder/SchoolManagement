using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
{
    public void Configure(EntityTypeBuilder<Guardian> builder)
    {
        builder.ToTable("Guardians");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.FullName)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.PhoneNumber)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(x => x.PhoneNumber);

        builder.Property(x => x.Email)
               .HasMaxLength(150);

        builder.HasIndex(x => x.Email)
               .IsUnique()
               .HasFilter("[Email] IS NOT NULL");

        builder.Property(x => x.Address)
               .HasMaxLength(500);

        builder.Property(x => x.Occupation)
               .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);
    }
}