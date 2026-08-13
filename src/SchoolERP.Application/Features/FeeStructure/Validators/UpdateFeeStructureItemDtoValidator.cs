using FluentValidation;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeStructure.Validators
{
    public class UpdateFeeStructureItemDtoValidator : AbstractValidator<UpdateFeeStructureItemDto>
    {
        public UpdateFeeStructureItemDtoValidator()
        {
            RuleFor(x => x.FeeTypeId).GreaterThan(0);
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);

            RuleFor(x => x)
                .Must(x => !(x.Id == null && x.IsDeleted))
                .WithMessage("A new item cannot be marked as deleted.")
                .WithName("IsDeleted");
        }
    }
}
