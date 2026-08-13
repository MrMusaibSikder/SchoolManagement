using FluentValidation;
using SchoolERP.Application.Features.Payment.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Payment.Validators
{
    public class VoidPaymentDtoValidator : AbstractValidator<VoidPaymentDto>
    {
        public VoidPaymentDtoValidator()
        {
            RuleFor(x => x.Reason).NotEmpty().MaximumLength(300);
        }
    }
}
