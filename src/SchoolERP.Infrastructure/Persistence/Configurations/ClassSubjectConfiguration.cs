using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class ClassSubjectConfiguration : IEntityTypeConfiguration<ClassSubject>
{
    public void Configure(EntityTypeBuilder<ClassSubject> builder)
    {
        builder.ToTable("ClassSubjects");

        // Composite Primary Key
        builder.HasKey(x => new { x.ClassId, x.SubjectId });

        builder.Property(x => x.IsOptional).HasDefaultValue(false);

        // Relationship: SchoolClass -> ClassSubjects
        builder.HasOne(x => x.SchoolClass)
               .WithMany(x => x.ClassSubjects)
               .HasForeignKey(x => x.ClassId)
               .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Subject -> ClassSubjects
        builder.HasOne(x => x.Subject)
               .WithMany(x => x.ClassSubjects)
               .HasForeignKey(x => x.SubjectId)
               .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(x => x.SubjectId);
    }
}