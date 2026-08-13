using FluentValidation;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeStructure.Validators
{
    public class CreateFeeStructureItemDtoValidator : AbstractValidator<CreateFeeStructureItemDto>
    {
        public CreateFeeStructureItemDtoValidator()
        {
            RuleFor(x => x.FeeTypeId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        }
    }
}
