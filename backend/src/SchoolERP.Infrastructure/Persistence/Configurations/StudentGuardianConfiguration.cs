using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class StudentGuardianConfiguration : IEntityTypeConfiguration<StudentGuardian>
{
    public void Configure(EntityTypeBuilder<StudentGuardian> builder)
    {
        builder.ToTable("StudentGuardians");

        // Composite Primary Key
        builder.HasKey(x => new { x.StudentId, x.GuardianId });

        // Properties
        builder.Property(x => x.Relation)
               .IsRequired()
               .HasMaxLength(50);

        // Relationship: Student
        builder.HasOne(x => x.Student)
               .WithMany(x => x.StudentGuardians)
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Guardian
        builder.HasOne(x => x.Guardian)
               .WithMany(x => x.StudentGuardians)
               .HasForeignKey(x => x.GuardianId)
               .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(x => x.GuardianId);
    }
}