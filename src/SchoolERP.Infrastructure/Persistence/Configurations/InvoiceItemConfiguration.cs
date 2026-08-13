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
    public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
    {
        public void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            builder.ToTable("InvoiceItems");

            // Primary Key
            builder.HasKey(x => x.Id);


            // Properties
            builder.Property(x => x.Description)
                   .IsRequired()
                   .HasMaxLength(200);


            builder.Property(x => x.OriginalAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();


            builder.Property(x => x.DiscountAmount)
                   .HasPrecision(18, 2)
                   .HasDefaultValue(0);


            builder.Property(x => x.FineAmount)
                   .HasPrecision(18, 2)
                   .HasDefaultValue(0);


            builder.Property(x => x.NetAmount)
                   .HasPrecision(18, 2)
                   .IsRequired();


            builder.Property(x => x.Quantity)
                   .HasDefaultValue(1);


            builder.Property(x => x.SortOrder)
                   .HasDefaultValue(0);



            // Relationships

            builder.HasOne(x => x.Invoice)
                   .WithMany(x => x.InvoiceItems)
                   .HasForeignKey(x => x.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(x => x.FeeType)
                   .WithMany(x => x.InvoiceItems)
                   .HasForeignKey(x => x.FeeTypeId)
                   .OnDelete(DeleteBehavior.Restrict);



            // Performance Indexes

            builder.HasIndex(x => x.InvoiceId)
                   .HasDatabaseName("IX_InvoiceItems_InvoiceId");


            builder.HasIndex(x => x.FeeTypeId)
                   .HasDatabaseName("IX_InvoiceItems_FeeTypeId");
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
