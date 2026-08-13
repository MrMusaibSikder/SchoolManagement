using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class EmployeeAttendanceConfiguration : IEntityTypeConfiguration<EmployeeAttendance>
{
    public void Configure(EntityTypeBuilder<EmployeeAttendance> builder)
    {
        builder.ToTable("EmployeeAttendances");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.AttendanceDate)
               .IsRequired();

        builder.Property(x => x.Status)
               .IsRequired();

        builder.Property(x => x.CheckIn);

        builder.Property(x => x.CheckOut);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationship
        builder.HasOne(x => x.Employee)
               .WithMany(x => x.Attendances)
               .HasForeignKey(x => x.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.EmployeeId);

        builder.HasIndex(x => new { x.EmployeeId, x.AttendanceDate })
               .IsUnique();
    }
}