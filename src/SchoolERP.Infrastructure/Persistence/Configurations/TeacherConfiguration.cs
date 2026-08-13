using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
{
    public void Configure(EntityTypeBuilder<Teacher> builder)
    {
        builder.ToTable("Teachers");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Qualification)
               .HasMaxLength(200);

        builder.Property(x => x.Specialization)
               .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // EmployeeId must be unique (One-to-One)
        builder.HasIndex(x => x.EmployeeId)
               .IsUnique();

        // Teacher -> SubjectTeachers (One-to-Many)
        builder.HasMany(x => x.SubjectTeachers)
               .WithOne(x => x.Teacher)
               .HasForeignKey(x => x.TeacherId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}