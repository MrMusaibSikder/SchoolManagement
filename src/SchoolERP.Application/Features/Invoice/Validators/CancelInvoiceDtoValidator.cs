using FluentValidation;
using SchoolERP.Application.Features.Invoice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.Validators
{
    public class CancelInvoiceDtoValidator : AbstractValidator<CancelInvoiceDto>
    {
        public CancelInvoiceDtoValidator()
        {
            RuleFor(x => x.CancellationReason)
                .NotEmpty().WithMessage("A cancellation reason is required.")
                .MaximumLength(300);
        }
    }
}
