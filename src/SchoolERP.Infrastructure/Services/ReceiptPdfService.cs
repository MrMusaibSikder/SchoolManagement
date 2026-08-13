using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Receipt.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class ReceiptPdfService : IReceiptPdfService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReceiptPdfService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<byte[]> GenerateReceiptPdfAsync(int receiptId, CancellationToken cancellationToken = default)
        {
            var receipt = await _unitOfWork.ReceiptRepository.GetWithDetailsAsync(receiptId, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.Receipt), receiptId);

            var payment = await _unitOfWork.PaymentRepository.GetWithDetailsAsync(receipt.PaymentId, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.Payment), receipt.PaymentId);

            var invoice = await _unitOfWork.InvoiceRepository.GetWithDetailsAsync(payment.InvoiceId, cancellationToken)
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.Invoice), payment.InvoiceId);

            var schools = await _unitOfWork.SchoolRepository.GetAllAsync(cancellationToken);
            var school = schools.FirstOrDefault(); // single-school system assumption

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().AlignCenter().Text(school?.Name ?? "School Name").FontSize(16).Bold();
                        if (!string.IsNullOrEmpty(school?.Address))
                            col.Item().AlignCenter().Text(school.Address).FontSize(9);
                        col.Item().PaddingTop(5).AlignCenter().Text("PAYMENT RECEIPT").FontSize(12).SemiBold();
                        col.Item().PaddingTop(8).LineHorizontal(1);
                    });

                    page.Content().PaddingTop(10).Column(col =>
                    {
                        // Receipt meta row
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Receipt No: {receipt.ReceiptNo}").SemiBold();
                            row.RelativeItem().AlignRight().Text($"Date: {receipt.IssuedAt:dd-MMM-yyyy hh:mm tt}");
                        });

                        col.Item().PaddingTop(4).Row(row =>
                        {
                            row.RelativeItem().Text($"Invoice No: {invoice.InvoiceNumber}");
                            row.RelativeItem().AlignRight().Text($"Payment No: {payment.PaymentNumber}");
                        });

                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        // Student info
                        col.Item().PaddingTop(8).Text("Student Details").Bold();
                        col.Item().Text($"Name: {invoice.Student.FullName}");
                        col.Item().Text($"Admission No: {invoice.Student.AdmissionNumber}");

                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        // Payment table
                        col.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                            });

                            table.Cell().Text("Description").Bold();
                            table.Cell().AlignRight().Text("Amount").Bold();

                            table.Cell().Text("Amount Paid");
                            table.Cell().AlignRight().Text($"{payment.Amount:N2}");

                            table.Cell().Text("Payment Method");
                            table.Cell().AlignRight().Text(payment.Method.ToString());

                            if (!string.IsNullOrEmpty(payment.TransactionId))
                            {
                                table.Cell().Text("Transaction ID");
                                table.Cell().AlignRight().Text(payment.TransactionId);
                            }
                        });

                        col.Item().PaddingTop(10).LineHorizontal(0.5f);

                        // Invoice balance (current, not historical snapshot — see caveat below)
                        col.Item().PaddingTop(8).Row(row =>
                        {
                            row.RelativeItem().Text("Invoice Total").SemiBold();
                            row.RelativeItem().AlignRight().Text($"{invoice.TotalAmount:N2}");
                        });
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Current Balance Due").SemiBold();
                            row.RelativeItem().AlignRight().Text($"{invoice.BalanceDue:N2}");
                        });

                        col.Item().PaddingTop(20).Row(row =>
                        {
                            row.RelativeItem().Text($"Collected By: {payment.CollectedByEmployee?.FullName ?? "-"}");
                            row.RelativeItem().AlignRight().Text("Signature: ______________");
                        });

                        if (receipt.IsVoided)
                        {
                            col.Item().PaddingTop(15).AlignCenter()
                                .Text("*** THIS RECEIPT HAS BEEN VOIDED ***")
                                .FontColor(Colors.Red.Medium).Bold();
                        }
                    });

                    page.Footer().AlignCenter().Text("This is a computer-generated receipt.").FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            return document.GeneratePdf();
        }
    }
}
