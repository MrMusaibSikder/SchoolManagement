using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolERP.Domain.Entities;


namespace SchoolERP.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            // Primary Key
            builder.HasKey(x => x.Id);

            // Properties

            builder.Property(x => x.PaymentNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(x => x.PaymentDate)
                   .IsRequired();

            builder.Property(x => x.Method)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(x => x.TransactionId)
                   .HasMaxLength(100);

            builder.Property(x => x.Remarks)
                   .HasMaxLength(500);

            // Relationships

            builder.HasOne(x => x.Invoice)
                   .WithMany(x => x.Payments)
                   .HasForeignKey(x => x.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Student)
                   .WithMany(x => x.Payments)
                   .HasForeignKey(x => x.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            // PaymentConfiguration.cs — ADD
            builder.HasOne(x => x.CollectedByEmployee)
                   .WithMany()
                   .HasForeignKey(x => x.CollectedByEmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
            // Unique Constraints

            builder.HasIndex(x => x.PaymentNumber)
                   .IsUnique()
                   .HasDatabaseName("IX_Payments_PaymentNumber");

            builder.HasIndex(x => x.TransactionId)
                   .IsUnique()
                   .HasFilter("[TransactionId] IS NOT NULL")
                   .HasDatabaseName("IX_Payments_TransactionId");

            // Performance Indexes
            builder.HasIndex(x => x.InvoiceId)
                   .HasDatabaseName("IX_Payments_InvoiceId");

            builder.HasIndex(x => x.StudentId)
                   .HasDatabaseName("IX_Payments_StudentId");

            builder.HasIndex(x => x.PaymentDate)
                   .HasDatabaseName("IX_Payments_PaymentDate");

            builder.HasIndex(x => x.CollectedByEmployeeId)
                   .HasDatabaseName("IX_Payments_CollectedByEmployeeId");

            // No Soft Delete
            // Financial records should be cancelled/voided, not deleted
        }
    }
}
