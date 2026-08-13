using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class EmployeeSalaryConfiguration : IEntityTypeConfiguration<EmployeeSalary>
{
    public void Configure(EntityTypeBuilder<EmployeeSalary> builder)
    {
        builder.ToTable("EmployeeSalaries");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.BasicSalary)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(x => x.EffectiveFrom)
               .IsRequired();

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationship
        builder.HasOne(x => x.Employee)
               .WithMany(x => x.Salaries)
               .HasForeignKey(x => x.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

        // Index
        builder.HasIndex(x => x.EmployeeId);
    }
}