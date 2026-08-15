using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CourseHub.Infrastructure.Persistence.Configurations;

public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.ToTable("Institutions");

        builder.ConfigureBaseEntity();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(i => i.Slug)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(i => i.Slug)
            .IsUnique();

        builder.Property(i => i.LogoUrl)
            .HasMaxLength(500);

        builder.Property(i => i.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(i => i.Description)
            .HasColumnType("text");

        builder.Property(i => i.Address)
            .HasMaxLength(300);

        builder.Property(i => i.Phone)
            .HasMaxLength(30);

        builder.Property(i => i.Email)
            .HasMaxLength(255);

        builder.Property(i => i.Website)
            .HasMaxLength(255);

        builder.Property(i => i.IsPublic)
            .IsRequired();

        builder.Property(i => i.IsActive)
            .IsRequired();
    }
}
