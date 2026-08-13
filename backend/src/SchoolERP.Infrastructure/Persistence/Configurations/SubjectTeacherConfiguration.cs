using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class SubjectTeacherConfiguration : IEntityTypeConfiguration<SubjectTeacher>
{
    public void Configure(EntityTypeBuilder<SubjectTeacher> builder)
    {
        builder.ToTable("SubjectTeachers");

        // Composite Primary Key
        builder.HasKey(x => new { x.SubjectId, x.TeacherId });

        // Relationship: Subject -> SubjectTeachers
        builder.HasOne(x => x.Subject)
               .WithMany(x => x.SubjectTeachers)
               .HasForeignKey(x => x.SubjectId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Teacher -> SubjectTeachers
        builder.HasOne(x => x.Teacher)
               .WithMany(x => x.SubjectTeachers)
               .HasForeignKey(x => x.TeacherId)
               .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(x => x.TeacherId);
    }
}