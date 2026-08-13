using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Payment.DTOs;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.Validators
{
    public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.InvoiceId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.InvoiceRepository.ExistsAsync(id, ct))
                .WithMessage("Selected invoice does not exist.");

            RuleFor(x => x.StudentId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.StudentRepository.ExistsAsync(id, ct))
                .WithMessage("Selected student does not exist.");

            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Payment amount must be greater than zero.");

            RuleFor(x => x.PaymentDate)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
                .WithMessage("Payment date cannot be in the future.");

            RuleFor(x => x.Method).IsInEnum();

            RuleFor(x => x.TransactionId)
                .NotEmpty().WithMessage("Transaction ID is required for non-cash payment methods.")
                .When(x => x.Method != PaymentMethod.Cash);

            RuleFor(x => x.TransactionId)
                .MaximumLength(100)
                .MustAsync(async (txId, ct) => string.IsNullOrEmpty(txId) || !await unitOfWork.PaymentRepository.TransactionIdExistsAsync(txId, ct))
                .WithMessage("This transaction ID has already been recorded.")
                .When(x => !string.IsNullOrEmpty(x.TransactionId));

            RuleFor(x => x.Remarks).MaximumLength(500);

            RuleFor(x => x)
                .MustAsync(async (dto, ct) =>
                {
                    var invoice = await unitOfWork.InvoiceRepository.GetByIdAsync(dto.InvoiceId, ct);
                    return invoice != null && dto.Amount <= invoice.BalanceDue;
                })
                .WithMessage("Payment amount exceeds the outstanding balance due on this invoice.")
                .WithName("Amount");
        }
    }
}
