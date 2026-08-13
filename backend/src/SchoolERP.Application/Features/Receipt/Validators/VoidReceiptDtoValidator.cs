using FluentValidation;
using SchoolERP.Application.Features.Receipt.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Receipt.Validators
{
    public class VoidReceiptDtoValidator : AbstractValidator<VoidReceiptDto>
    {
        public VoidReceiptDtoValidator()
        {
            RuleFor(x => x.VoidReason).NotEmpty().MaximumLength(300);
        }
    }
}
