using FluentValidation;
using SchoolERP.Application.Features.LateFineRule.DTOs;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.LateFineRule.Validators
{
    public class UpdateLateFineRuleDtoValidator : AbstractValidator<UpdateLateFineRuleDto>
    {
        public UpdateLateFineRuleDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Type).IsInEnum();
            RuleFor(x => x.Amount).GreaterThan(0);

            RuleFor(x => x.Amount)
                .InclusiveBetween(0.01m, 100)
                .When(x => x.Type == FineType.Percentage);

            RuleFor(x => x.GracePeriodDays).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxFineAmount).GreaterThan(0).When(x => x.MaxFineAmount.HasValue);
        }
    }
}
