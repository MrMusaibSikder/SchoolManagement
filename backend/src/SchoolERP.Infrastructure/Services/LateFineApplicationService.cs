using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Invoice.DTOs;
using SchoolERP.Application.Features.Invoice.Interfaces;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class LateFineApplicationService : ILateFineApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LateFineApplicationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LateFineApplicationResultDto> ApplyLateFinesAsync(
            DateTime? asOfDate = null, CancellationToken cancellationToken = default)
        {
            var effectiveDate = asOfDate ?? DateTime.UtcNow.Date;
            var result = new LateFineApplicationResultDto();

            // Light query first — শুধু Id/DueDate/AcademicYearId, item detail ছাড়া
            var overdueInvoices = await _unitOfWork.InvoiceRepository.GetOverdueInvoicesAsync(effectiveDate, cancellationToken);
            result.TotalInvoicesEvaluated = overdueInvoices.Count;

            foreach (var summary in overdueInvoices)
            {
                try
                {
                    // প্রতিটা invoice আলাদা করে tracked+items সহ আনা হচ্ছে —
                    // একসাথে সব invoice tracked অবস্থায় memory তে রাখলে বড় স্কুলে ChangeTracker ভারী হয়ে যাবে
                    var invoice = await _unitOfWork.InvoiceRepository.GetTrackedWithItemsAsync(summary.Id, cancellationToken);
                    if (invoice is null) continue;

                    var daysOverdue = (effectiveDate.Date - invoice.DueDate.Date).Days;
                    if (daysOverdue <= 0) continue; // safety guard, honestly GetOverdueInvoicesAsync already filters this

                    bool anyItemChanged = false;

                    foreach (var item in invoice.InvoiceItems)
                    {
                        var rule = await _unitOfWork.LateFineRuleRepository.GetApplicableRuleAsync(
                            invoice.AcademicYearId, item.FeeTypeId, cancellationToken);

                        if (rule is null)
                        {
                            result.SkippedNoRule++;
                            continue;
                        }

                        var effectiveDaysOverdue = daysOverdue - rule.GracePeriodDays;
                        if (effectiveDaysOverdue <= 0)
                        {
                            result.SkippedWithinGracePeriod++;
                            continue;
                        }

                        var calculatedFine = rule.Type switch
                        {
                            FineType.Fixed => rule.Amount,
                            FineType.Percentage => Math.Round(item.OriginalAmount * rule.Amount / 100m, 2),
                            FineType.DailyAccrual => Math.Round(rule.Amount * effectiveDaysOverdue, 2),
                            _ => 0m
                        };

                        if (rule.MaxFineAmount.HasValue)
                            calculatedFine = Math.Min(calculatedFine, rule.MaxFineAmount.Value);

                        // শুধু আসলেই বদলালে touch করা হচ্ছে — অপ্রয়োজনীয় UPDATE এড়ানোর জন্য
                        if (item.FineAmount != calculatedFine)
                        {
                            result.TotalFineApplied += calculatedFine - item.FineAmount;
                            item.FineAmount = calculatedFine;
                            item.NetAmount = item.OriginalAmount - item.DiscountAmount + item.FineAmount;
                            item.UpdatedAt = DateTime.UtcNow;
                            anyItemChanged = true;
                        }
                    }

                    if (anyItemChanged)
                    {
                        invoice.TotalAmount = invoice.InvoiceItems.Sum(i => i.NetAmount * i.Quantity);
                        invoice.BalanceDue = invoice.TotalAmount - invoice.AmountPaid;

                        // 🎯 সরলীকৃত সিদ্ধান্ত: Status Issued/PartiallyPaid থেকে Overdue করে দেওয়া হচ্ছে,
                        // যাতে UI-তে আলাদা করে ফিল্টার করা যায়। ভবিষ্যতে payment এলে PaymentService আবার
                        // Paid/PartiallyPaid এ ফিরিয়ে দেবে (Overdue flag payment এর সময় "ভুলে" যাবে) —
                        // এটা একটা known trade-off, পুরোপুরি state machine না বানিয়ে simple রাখা হয়েছে।
                        if (invoice.Status == InvoiceStatus.Issued || invoice.Status == InvoiceStatus.PartiallyPaid)
                            invoice.Status = InvoiceStatus.Overdue;

                        invoice.UpdatedAt = DateTime.UtcNow;

                        try
                        {
                            await _unitOfWork.SaveChangesAsync(cancellationToken);
                            result.InvoicesUpdated++;
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
                        {
                            // এই মুহূর্তে কেউ payment collect করছে হয়তো — এই invoice skip করে পরেরটায় যাওয়া হচ্ছে,
                            // পরের দিনের রান এ আবার চেষ্টা হবে
                            result.Failed++;
                            result.Errors.Add(new InvoiceGenerationErrorDto
                            {
                                StudentId = invoice.StudentId,
                                StudentName = string.Empty,
                                Reason = $"Invoice #{invoice.InvoiceNumber}: concurrency conflict, skipped this run."
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.Failed++;
                    result.Errors.Add(new InvoiceGenerationErrorDto
                    {
                        StudentId = summary.StudentId,
                        StudentName = string.Empty,
                        Reason = $"Invoice #{summary.InvoiceNumber}: {ex.Message}"
                    });
                }
            }

            return result;
        }
    }
}
