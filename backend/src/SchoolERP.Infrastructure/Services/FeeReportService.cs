using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeReports.DTOs;
using SchoolERP.Application.Features.FeeReports.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class FeeReportService : IFeeReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeeReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FeeCollectionSummaryDto> GetCollectionSummaryAsync(
            DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
        {
            if (dateTo < dateFrom)
                throw new BadRequestException("dateTo cannot be earlier than dateFrom.");

            // 🎯 dateTo-কে দিনের শেষ পর্যন্ত extend করা হচ্ছে — নাহলে "2026-08-04" পাঠালে
            // ওইদিনের payment গুলো বাদ পড়ে যাবে (কারণ PaymentDate তে time component থাকতে পারে)
            var inclusiveTo = dateTo.Date.AddDays(1).AddTicks(-1);

            var payments = await _unitOfWork.PaymentRepository.GetCompletedForReportAsync(
                dateFrom.Date, inclusiveTo, cancellationToken);

            var dailyBreakdown = payments
                .GroupBy(p => p.PaymentDate.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyCollectionPointDto
                {
                    Date = g.Key,
                    Amount = g.Sum(p => p.Amount),
                    TransactionCount = g.Count()
                })
                .ToList();

            var methodBreakdown = payments
                .GroupBy(p => p.Method)
                .OrderByDescending(g => g.Sum(p => p.Amount))
                .Select(g => new MethodCollectionPointDto
                {
                    Method = g.Key.ToString(),
                    Amount = g.Sum(p => p.Amount),
                    TransactionCount = g.Count()
                })
                .ToList();

            var totalCollected = payments.Sum(p => p.Amount);
            var totalTransactions = payments.Count;

            return new FeeCollectionSummaryDto
            {
                DateFrom = dateFrom.Date,
                DateTo = dateTo.Date,
                TotalCollected = totalCollected,
                TotalTransactions = totalTransactions,
                AverageTransactionAmount = totalTransactions == 0 ? 0 : Math.Round(totalCollected / totalTransactions, 2),
                DailyBreakdown = dailyBreakdown,
                MethodBreakdown = methodBreakdown
            };
        }

        public async Task<DefaulterReportDto> GetDefaulterReportAsync(
            DateTime? asOfDate = null, int? schoolClassId = null, CancellationToken cancellationToken = default)
        {
            var effectiveDate = asOfDate ?? DateTime.UtcNow.Date;

            var overdueInvoices = await _unitOfWork.InvoiceRepository.GetOverdueWithStudentDetailsAsync(
                effectiveDate, cancellationToken);

            // 🎯 Optional class filter — repository এ পাঠাইনি কারণ GetOverdueWithStudentDetailsAsync()
            // ইতিমধ্যেই generic (late-fine job এও future এ reuse হতে পারে); filter এখানে in-memory করাই
            // যথেষ্ট, কারণ overdue invoice সংখ্যা সাধারণত hundreds এর মধ্যেই থাকে, বড় dataset না
            if (schoolClassId.HasValue)
                overdueInvoices = overdueInvoices.Where(x => x.Student.ClassId == schoolClassId.Value).ToList();

            var defaulters = overdueInvoices
                .GroupBy(x => x.StudentId)
                .Select(g =>
                {
                    var student = g.First().Student;
                    var oldestDue = g.Min(x => x.DueDate);

                    return new DefaulterDto
                    {
                        StudentId = student.Id,
                        StudentName = student.FullName,
                        AdmissionNumber = student.AdmissionNumber,
                        ClassName = student.SchoolClass?.Name ?? string.Empty,
                        SectionName = student.Section?.Name,
                        OverdueInvoiceCount = g.Count(),
                        TotalOverdueAmount = g.Sum(x => x.BalanceDue),
                        OldestDueDate = oldestDue,
                        DaysOverdue = (effectiveDate.Date - oldestDue.Date).Days
                    };
                })
                .OrderByDescending(d => d.TotalOverdueAmount)
                .ToList();

            return new DefaulterReportDto
            {
                AsOfDate = effectiveDate,
                TotalDefaulters = defaulters.Count,
                TotalOverdueAmount = defaulters.Sum(d => d.TotalOverdueAmount),
                Defaulters = defaulters
            };
        }
    }
}
