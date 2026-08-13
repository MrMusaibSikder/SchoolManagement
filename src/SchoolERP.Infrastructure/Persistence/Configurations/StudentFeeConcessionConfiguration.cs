using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Persistence.Configurations
{
    public class StudentFeeConcessionConfiguration : IEntityTypeConfiguration<StudentFeeConcession>
    {
        public void Configure(EntityTypeBuilder<StudentFeeConcession> builder)
        {
            builder.ToTable("StudentFeeConcessions");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties
            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(30);

            builder.Property(x => x.Value)
                   .HasPrecision(18, 2)
                   ;
            builder.Property(x => x.ValidFrom);
            builder.Property(x => x.ValidTo);

            builder.Property(x => x.Reason)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(x => x.RequiresApproval)
                   .HasDefaultValue(false);

            builder.Property(x => x.IsApproved)
                   .HasDefaultValue(false);

            builder.Property(x => x.IsActive)
                   .HasDefaultValue(true);

            // Relationships
            builder.HasOne(x => x.Student)
                   .WithMany(x => x.FeeConcessions)
                   .HasForeignKey(x => x.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FeeType)
                   .WithMany(x => x.StudentFeeConcessions)
                   .HasForeignKey(x => x.FeeTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AcademicYear)
                   .WithMany(x => x.StudentFeeConcessions)
                   .HasForeignKey(x => x.AcademicYearId)
                   .OnDelete(DeleteBehavior.Restrict);
            // StudentFeeConcessionConfiguration.cs — ADD
            builder.HasOne(x => x.ApprovedByEmployee)
                   .WithMany()
                   .HasForeignKey(x => x.ApprovedByEmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Unique Constraint
            builder.HasIndex(x => new
            {
                x.StudentId,
                x.FeeTypeId,
                x.AcademicYearId
            })
            .IsUnique()
            .HasDatabaseName("IX_StudentFeeConcessions_Student_FeeType_AcademicYear");

            // Performance Indexes
            builder.HasIndex(x => x.StudentId)
                   .HasDatabaseName("IX_StudentFeeConcessions_StudentId");

            builder.HasIndex(x => x.FeeTypeId)
                   .HasDatabaseName("IX_StudentFeeConcessions_FeeTypeId");

            builder.HasIndex(x => x.AcademicYearId)
                   .HasDatabaseName("IX_StudentFeeConcessions_AcademicYearId");

            builder.HasIndex(x => x.IsApproved)
                   .HasDatabaseName("IX_StudentFeeConcessions_IsApproved");

            // Soft Delete
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
