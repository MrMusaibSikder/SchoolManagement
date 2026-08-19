using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.ToTable("PasswordResetTokens");

        builder.ConfigureBaseEntity();

        builder.Property(prt => prt.UserId)
            .IsRequired();

        builder.Property(prt => prt.TokenHash)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(prt => prt.TokenHash)
            .IsUnique();

        builder.HasIndex(prt => prt.UserId);

        builder.Property(prt => prt.ExpiresAt)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(prt => prt.UsedAt)
            .HasColumnType("timestamptz");

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(prt => prt.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
