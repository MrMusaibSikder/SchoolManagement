using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Invoice.DTOs;
using SchoolERP.Application.Features.Invoice.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class InvoiceGenerationService : IInvoiceGenerationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GenerateMonthlyInvoicesDto> _validator;

        public InvoiceGenerationService(IUnitOfWork unitOfWork, IValidator<GenerateMonthlyInvoicesDto> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<InvoiceGenerationResultDto> GenerateMonthlyInvoicesAsync(
            GenerateMonthlyInvoicesDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var result = new InvoiceGenerationResultDto();

            // 🎯 শুধু active fee structure যেগুলো এই academic year + (optionally) class-এর জন্য প্রযোজ্য
            var feeStructures = await _unitOfWork.FeeStructureRepository.GetListAsync(
                request.AcademicYearId, request.SchoolClassId, isActive: true, cancellationToken);

            foreach (var structure in feeStructures)
            {
                // Detailed structure (Items + FeeType) লাগবে Frequency চেক করার জন্য
                var fullStructure = await _unitOfWork.FeeStructureRepository.GetWithItemsAsync(structure.Id, cancellationToken);
                if (fullStructure is null) continue;

                var monthlyItems = fullStructure.FeeStructureItems
                    .Where(i => i.FeeType.Frequency == FeeFrequency.Monthly)
                    .ToList();

                if (monthlyItems.Count == 0)
                    continue; // এই structure এ কোনো monthly fee নেই (হয়তো শুধু Termly/Yearly) — এই run এ skip

                var students = await _unitOfWork.StudentRepository.GetActiveByClassSectionAsync(
                    fullStructure.SchoolClassId, fullStructure.SectionId, cancellationToken);

                result.TotalStudentsEvaluated += students.Count;

                foreach (var student in students)
                {
                    try
                    {
                        var alreadyInvoiced = await _unitOfWork.InvoiceRepository.ExistsForPeriodAsync(
                            student.Id, request.AcademicYearId, request.Month, request.Year, fullStructure.Id, cancellationToken);

                        if (alreadyInvoiced)
                        {
                            result.SkippedAlreadyInvoiced++;
                            continue;
                        }

                        var invoiceItems = new List<InvoiceItem>();

                        foreach (var item in monthlyItems)
                        {
                            var (discountAmount, _) = await ResolveConcessionAsync(
                                student.Id, item.FeeTypeId, request.AcademicYearId, item.Amount, cancellationToken);

                            invoiceItems.Add(new InvoiceItem
                            {
                                FeeTypeId = item.FeeTypeId,
                                Description = item.FeeType.Name,
                                OriginalAmount = item.Amount,
                                DiscountAmount = discountAmount,
                                FineAmount = 0, // Late fine এখানে যোগ হয় না — সেটা আলাদা "Late Fine Job"-এর দায়িত্ব (পরের ফিচার)
                                NetAmount = item.Amount - discountAmount,
                                Quantity = 1,
                                SortOrder = item.SortOrder
                            });
                        }

                        var totalAmount = invoiceItems.Sum(i => i.NetAmount * i.Quantity);

                        var invoice = new SchoolERP.Domain.Entities.Invoice
                        {
                            InvoiceNumber = await GenerateInvoiceNumberAsync(request.AcademicYearId, cancellationToken),
                            AcademicYearId = request.AcademicYearId,
                            StudentId = student.Id,
                            FeeStructureId = fullStructure.Id,
                            Status = InvoiceStatus.Issued,
                            InvoiceDate = request.InvoiceDate ?? DateTime.UtcNow.Date,
                            DueDate = request.DueDate,
                            Month = request.Month,
                            Year = request.Year,
                            TotalAmount = totalAmount,
                            AmountPaid = 0,
                            BalanceDue = totalAmount,
                            IsAutoGenerated = true,
                            InvoiceItems = invoiceItems
                        };

                        await _unitOfWork.InvoiceRepository.AddAsync(invoice, cancellationToken);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        result.InvoicesCreated++;
                    }
                    catch (Exception ex)
                    {
                        // 🎯 একজন student এর জন্য কিছু ভুল হলে বাকিদের generation থামবে না —
                        // error report করে পরের student এ চলে যাওয়া হয়, পুরো batch fail করানো হয় না
                        result.Failed++;
                        result.Errors.Add(new InvoiceGenerationErrorDto
                        {
                            StudentId = student.Id,
                            StudentName = student.FullName,
                            Reason = ex.Message
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Resolves the active, approved concession (if any) for this student/fee-type/year and
        /// converts it into a discount amount against the given base amount.
        /// </summary>
        private async Task<(decimal DiscountAmount, bool HasConcession)> ResolveConcessionAsync(
            int studentId, int feeTypeId, int academicYearId, decimal baseAmount, CancellationToken cancellationToken)
        {
            var concession = await _unitOfWork.StudentFeeConcessionRepository.GetActiveForStudentFeeTypeAsync(
                studentId, feeTypeId, academicYearId, cancellationToken);

            if (concession is null) return (0, false);

            var discount = concession.Type switch
            {
                ConcessionType.FullExemption => baseAmount,
                ConcessionType.PercentageDiscount => Math.Round(baseAmount * (concession.Value ?? 0) / 100m, 2),
                ConcessionType.FixedAmountDiscount => Math.Min(concession.Value ?? 0, baseAmount),
                _ => 0m
            };

            return (discount, true);
        }

        /// <summary>
        /// ⚠️ InvoiceService.GenerateInvoiceNumberAsync() এর সাথে হুবহু ডুপ্লিকেট লজিক।
        /// TODO: ভবিষ্যতে একটা shared IInvoiceNumberGenerator বানিয়ে দুই জায়গা থেকে এটা বের করে আনা উচিত,
        /// যাতে format বদলালে দুই জায়গায় আলাদা করে আপডেট করতে না হয়।
        /// </summary>
        private async Task<string> GenerateInvoiceNumberAsync(int academicYearId, CancellationToken cancellationToken)
        {
            var lastNumber = await _unitOfWork.InvoiceRepository.GetLastInvoiceNumberAsync(academicYearId, cancellationToken);

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastNumber))
            {
                var parts = lastNumber.Split('-');
                if (parts.Length > 0 && int.TryParse(parts[^1], out var lastSeq))
                    nextSequence = lastSeq + 1;
            }

            var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(academicYearId, cancellationToken);
            var yearPart = academicYear?.Name.Replace(" ", "").Replace("/", "-") ?? DateTime.UtcNow.Year.ToString();

            return $"INV-{yearPart}-{nextSequence:D6}";
        }
    }
}
