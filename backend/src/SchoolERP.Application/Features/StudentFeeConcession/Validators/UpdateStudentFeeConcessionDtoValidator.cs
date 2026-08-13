using FluentValidation;
using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.Validators
{
    public class UpdateStudentFeeConcessionDtoValidator : AbstractValidator<UpdateStudentFeeConcessionDto>
    {
        public UpdateStudentFeeConcessionDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Type).IsInEnum();

            RuleFor(x => x.Value)
                .NotNull()
                .When(x => x.Type != ConcessionType.FullExemption);

            RuleFor(x => x.Value)
                .InclusiveBetween(0.01m, 100)
                .When(x => x.Type == ConcessionType.PercentageDiscount && x.Value.HasValue);

            RuleFor(x => x.Reason).NotEmpty().MaximumLength(300);

            RuleFor(x => x.ValidTo)
                .GreaterThan(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue && x.ValidTo.HasValue);
        }
    }
}
