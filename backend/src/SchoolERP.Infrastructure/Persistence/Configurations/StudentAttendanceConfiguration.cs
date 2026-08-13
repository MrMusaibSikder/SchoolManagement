using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class StudentAttendanceConfiguration : IEntityTypeConfiguration<StudentAttendance>
{
    public void Configure(EntityTypeBuilder<StudentAttendance> builder)
    {
        builder.ToTable("StudentAttendances");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.StudentId)
               .IsRequired();


        builder.Property(x => x.AttendanceDate)
               .IsRequired();


        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();


        builder.Property(x => x.Remarks)
               .HasMaxLength(500);


        builder.Property(x => x.CreatedAt)
               .IsRequired();


        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);



        builder.HasOne(x => x.Student)
               .WithMany(x => x.Attendances)
               .HasForeignKey(x => x.StudentId)
               .OnDelete(DeleteBehavior.Restrict);



        builder.HasIndex(x => new
        {
            x.StudentId,
            x.AttendanceDate
        })
        .IsUnique();
    }
}