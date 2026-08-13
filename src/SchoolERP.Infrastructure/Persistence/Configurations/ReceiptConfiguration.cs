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
    public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
    {
        public void Configure(EntityTypeBuilder<Receipt> builder)
        {
            builder.ToTable("Receipts");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties

            builder.Property(x => x.ReceiptNo)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.IssuedAt)
                   .IsRequired();

            builder.Property(x => x.IsVoided)
                   .HasDefaultValue(false);


            builder.Property(x => x.VoidReason)
                   .HasMaxLength(300);

            // Relationship
            builder.HasOne(x => x.Payment)
                   .WithOne(x => x.Receipt)
                   .HasForeignKey<Receipt>(x => x.PaymentId)
                   .OnDelete(DeleteBehavior.Restrict);
            // ReceiptConfiguration.cs — ADD
            builder.HasOne(x => x.IssuedByEmployee)
                   .WithMany()
                   .HasForeignKey(x => x.IssuedByEmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Unique Constraints

            builder.HasIndex(x => x.ReceiptNo)
                   .IsUnique()
                   .HasDatabaseName("IX_Receipts_ReceiptNo");


            builder.HasIndex(x => x.PaymentId)
                   .IsUnique()
                   .HasDatabaseName("IX_Receipts_PaymentId");
            // Performance Index — ADD
            builder.HasIndex(x => x.IssuedByEmployeeId)
                   .HasDatabaseName("IX_Receipts_IssuedByEmployeeId");
            // No Soft Delete
            // Receipt should be voided, not deleted
        }
    }
}
