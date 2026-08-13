using AutoMapper;
using FluentValidation;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Features.Payment.DTOs;
using SchoolERP.Application.Features.Payment.Interfaces;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentEmployeeService _currentEmployee;
        private readonly IValidator<CreatePaymentDto> _createValidator;
        private readonly IValidator<VoidPaymentDto> _voidValidator;

        public PaymentService(
            IUnitOfWork unitOfWork, IMapper mapper, ICurrentEmployeeService currentEmployee,
            IValidator<CreatePaymentDto> createValidator, IValidator<VoidPaymentDto> voidValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentEmployee = currentEmployee;
            _createValidator = createValidator;
            _voidValidator = voidValidator;
        }

        public async Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var payment = await _unitOfWork.PaymentRepository.GetWithDetailsAsync(id, cancellationToken);
            return payment is null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<IReadOnlyList<PaymentListDto>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default)
        {
            var payments = await _unitOfWork.PaymentRepository.GetByInvoiceIdAsync(invoiceId, cancellationToken);
            return _mapper.Map<IReadOnlyList<PaymentListDto>>(payments);
        }

        /// <summary>
        /// Creates the Payment and Receipt, and updates the Invoice's AmountPaid,
        /// BalanceDue, and Status — all within the same database transaction,
        /// ensuring that no partial data is saved if any part of the process fails.
        /// </summary>
        public async Task<PaymentDto> CreateAsync(CreatePaymentDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            var employeeId = await _currentEmployee.GetIdAsync(cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // The Invoice is retrieved as a tracked entity — this is important for RowVersion concurrency control.
                var invoice = await _unitOfWork.InvoiceRepository.GetTrackedWithItemsAsync(request.InvoiceId, cancellationToken)
                    ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.Invoice), request.InvoiceId);

                if (invoice.Status == InvoiceStatus.Cancelled)
                    throw new BadRequestException("Cannot record a payment against a cancelled invoice.");

                // Double-check (the validator already performs this check, but this provides defense-in-depth against race conditions).
                if (request.Amount > invoice.BalanceDue)
                    throw new BadRequestException("Payment amount exceeds the outstanding balance due on this invoice.");

                var payment = new SchoolERP.Domain.Entities.Payment
                {
                    PaymentNumber = await GeneratePaymentNumberAsync(cancellationToken),
                    InvoiceId = request.InvoiceId,
                    StudentId = request.StudentId,
                    Amount = request.Amount,
                    PaymentDate = request.PaymentDate,
                    Method = request.Method,
                    Status = PaymentStatus.Completed,
                    TransactionId = request.TransactionId,
                    Remarks = request.Remarks,
                    CollectedByEmployeeId = employeeId
                };

                await _unitOfWork.PaymentRepository.AddAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // Payment.Id দরকার Receipt-এর FK-এর জন্য

                var receipt = new SchoolERP.Domain.Entities.Receipt
                {
                    ReceiptNo = await GenerateReceiptNoAsync(cancellationToken),
                    PaymentId = payment.Id,
                    IssuedAt = DateTime.UtcNow,
                    IssuedByEmployeeId = employeeId,
                    IsVoided = false
                };
                await _unitOfWork.ReceiptRepository.AddAsync(receipt, cancellationToken);

                // Update the Invoice balance — the entity is tracked, so EF will perform a partial UPDATE automatically (including RowVersion handling).
                invoice.AmountPaid += request.Amount;
                invoice.BalanceDue -= request.Amount;
                invoice.Status = invoice.BalanceDue <= 0 ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
                invoice.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var created = await _unitOfWork.PaymentRepository.GetWithDetailsAsync(payment.Id, cancellationToken);
                return _mapper.Map<PaymentDto>(created!);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new ConflictException("This invoice was modified by another payment at the same time. Please refresh and try again.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        /// <summary>
        /// When a payment is voided, the corresponding amount is restored to the Invoice balance,
        /// and the associated receipt is also voided.
        /// </summary>
        public async Task VoidAsync(int id, VoidPaymentDto request, CancellationToken cancellationToken = default)
        {
            var validation = await _voidValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var payment = await _unitOfWork.PaymentRepository.GetByIdTrackedAsync(id, cancellationToken)
                    ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.Payment), id);

                if (payment.Status == PaymentStatus.Voided)
                    throw new BadRequestException("Payment is already voided.");

                var invoice = await _unitOfWork.InvoiceRepository.GetTrackedWithItemsAsync(payment.InvoiceId, cancellationToken)
                    ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.Invoice), payment.InvoiceId);

                payment.Status = PaymentStatus.Voided;
                payment.Remarks = string.IsNullOrEmpty(payment.Remarks)
                    ? $"Voided: {request.Reason}"
                    : $"{payment.Remarks} | Voided: {request.Reason}";
                payment.UpdatedAt = DateTime.UtcNow;

                var receipt = await _unitOfWork.ReceiptRepository.GetByPaymentIdAsync(payment.InvoiceId, cancellationToken);
                if (receipt is not null)
                {
                    var trackedReceipt = await _unitOfWork.ReceiptRepository.GetByIdTrackedAsync(receipt.Id, cancellationToken);
                    if (trackedReceipt is not null)
                    {
                        trackedReceipt.IsVoided = true;
                        trackedReceipt.VoidedAt = DateTime.UtcNow;
                        trackedReceipt.VoidReason = request.Reason;
                    }
                }

                //return Invoice balance 
                invoice.AmountPaid -= payment.Amount;
                invoice.BalanceDue += payment.Amount;
                invoice.Status = invoice.AmountPaid <= 0
                    ? InvoiceStatus.Issued
                    : InvoiceStatus.PartiallyPaid;
                invoice.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new ConflictException("This invoice was modified by another user. Please refresh and try again.");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        private async Task<string> GeneratePaymentNumberAsync(CancellationToken cancellationToken)
        {
            var lastNumber = await _unitOfWork.PaymentRepository.GetLastPaymentNumberAsync(cancellationToken);
            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastNumber))
            {
                var parts = lastNumber.Split('-');
                if (parts.Length > 0 && int.TryParse(parts[^1], out var lastSeq))
                    nextSequence = lastSeq + 1;
            }
            return $"PAY-{DateTime.UtcNow:yyyyMM}-{nextSequence:D6}";
        }

        private async Task<string> GenerateReceiptNoAsync(CancellationToken cancellationToken)
        {
            var lastNumber = await _unitOfWork.ReceiptRepository.GetLastReceiptNoAsync(cancellationToken);
            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastNumber))
            {
                var parts = lastNumber.Split('-');
                if (parts.Length > 0 && int.TryParse(parts[^1], out var lastSeq))
                    nextSequence = lastSeq + 1;
            }
            return $"RCPT-{DateTime.UtcNow:yyyyMM}-{nextSequence:D6}";
        }
    }
}
