using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.AdmissionNumber)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(x => x.AdmissionNumber)
               .IsUnique();

        builder.Property(x => x.FullName)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.RollNo)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.Gender)
               .IsRequired()
               .HasMaxLength(10);

        builder.Property(x => x.BloodGroup)
               .HasMaxLength(5);

        builder.Property(x => x.Address)
               .HasMaxLength(500);

        builder.Property(x => x.Photo)
               .HasMaxLength(500);

        builder.Property(x => x.DateOfBirth)
               .IsRequired();

        builder.Property(x => x.AdmissionDate)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // One Class + One Section + One Roll = Unique
        builder.HasIndex(x => new { x.ClassId, x.SectionId, x.RollNo })
               .IsUnique();

        // Relationships
        builder.HasOne(x => x.SchoolClass)
               .WithMany()
               .HasForeignKey(x => x.ClassId)
               .IsRequired(true)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Section)
               .WithMany(x => x.Students)
               .HasForeignKey(x => x.SectionId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        // Check Constraints
        builder.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Student_Gender",
                "[Gender] IN ('Male','Female','Other')");
        });
    }
}