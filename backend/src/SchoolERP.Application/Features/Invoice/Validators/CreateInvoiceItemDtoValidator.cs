using FluentValidation;
using SchoolERP.Application.Features.Invoice.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Invoice.Validators
{
    public class CreateInvoiceItemDtoValidator : AbstractValidator<CreateInvoiceItemDto>
    {
        public CreateInvoiceItemDtoValidator()
        {
            RuleFor(x => x.FeeTypeId).GreaterThan(0);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
            RuleFor(x => x.OriginalAmount).GreaterThan(0);
            RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.FineAmount).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Quantity).GreaterThan(0);

            RuleFor(x => x)
                .Must(x => x.DiscountAmount <= x.OriginalAmount)
                .WithMessage("Discount amount cannot exceed the original amount.")
                .WithName("DiscountAmount");
        }
    }
}
