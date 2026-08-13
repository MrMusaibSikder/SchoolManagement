using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
               .IsRequired()
               .HasMaxLength(256);

        builder.HasIndex(x => x.Token)
               .IsUnique();

        builder.Property(x => x.CreatedByIp)
               .HasMaxLength(64);

        builder.Property(x => x.ReplacedByToken)
               .HasMaxLength(256);

        builder.Property(x => x.ExpiresAt)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // RefreshToken.IsActive is a computed, not-mapped convenience property.
        builder.Ignore(x => x.IsActive);

        // User -> RefreshTokens (One-to-Many). No inverse navigation on User,
        // so the FK is configured without WithMany(navigation).
        builder.HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
    }
}
