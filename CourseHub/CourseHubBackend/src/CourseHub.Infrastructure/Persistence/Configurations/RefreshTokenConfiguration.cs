using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.ConfigureBaseEntity();

        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.TokenHash)
            .IsRequired()
            .HasMaxLength(200);

        // Lookups happen by hash (validating an incoming refresh token),
        // so this must be indexed; unique because a hash collision would
        // otherwise let one stored row match two distinct raw tokens.
        builder.HasIndex(rt => rt.TokenHash)
            .IsUnique();

        // Common query: "all active sessions for this user" (reuse
        // detection, revoke-all-on-password-change).
        builder.HasIndex(rt => rt.UserId);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired()
            .HasColumnType("timestamptz");

        builder.Property(rt => rt.RevokedAt)
            .HasColumnType("timestamptz");

        builder.Property(rt => rt.ReplacedByTokenId);

        builder.Property(rt => rt.CreatedByIp)
            .HasMaxLength(45); // long enough for an IPv6 address

        // Restrict: a user's login history/session audit trail should not
        // silently disappear if the User row is ever removed.
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
