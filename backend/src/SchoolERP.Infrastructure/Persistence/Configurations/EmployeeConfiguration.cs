using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        // Primary Key
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.EmployeeCode)
               .IsRequired()
               .HasMaxLength(50);

        builder.HasIndex(x => x.EmployeeCode)
               .IsUnique();

        builder.Property(x => x.FullName)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.Phone)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(x => x.Email)
               .HasMaxLength(150);

        builder.HasIndex(x => x.Email)
               .IsUnique()
               .HasFilter("[Email] IS NOT NULL");

        builder.Property(x => x.JoiningDate)
               .IsRequired();

        builder.Property(x => x.IsActive)
               .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
               .IsRequired();

        builder.Property(x => x.IsDeleted)
               .HasDefaultValue(false);

        // Relationships

        // Employee -> Designation (Many-to-One)
        builder.HasOne(x => x.Designation)
               .WithMany(x => x.Employees)
               .HasForeignKey(x => x.DesignationId)
               .OnDelete(DeleteBehavior.Restrict);

        // Employee -> User (One-to-One)
        builder.HasOne(x => x.User)
               .WithOne(x => x.Employee)
               .HasForeignKey<Employee>(x => x.UserId)
               .OnDelete(DeleteBehavior.SetNull);

        // Employee -> Teacher (One-to-One)
        builder.HasOne(x => x.Teacher)
               .WithOne(x => x.Employee)
               .HasForeignKey<Teacher>(x => x.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

        // Employee -> Attendances (One-to-Many)
        builder.HasMany(x => x.Attendances)
               .WithOne(x => x.Employee)
               .HasForeignKey(x => x.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);

        // Employee -> Salaries (One-to-Many)
        builder.HasMany(x => x.Salaries)
               .WithOne(x => x.Employee)
               .HasForeignKey(x => x.EmployeeId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}