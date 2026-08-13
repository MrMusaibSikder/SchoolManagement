using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeStructure.Validators
{
    public class UpdateFeeStructureDtoValidator : AbstractValidator<UpdateFeeStructureDto>
    {
        public UpdateFeeStructureDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);

            RuleFor(x => x.SectionId)
                .MustAsync(async (id, ct) => id == null || await unitOfWork.SectionRepository.ExistsAsync(id.Value, ct))
                .WithMessage("Selected section does not exist.");

            RuleFor(x => x.EffectiveTo)
                .GreaterThan(x => x.EffectiveFrom)
                .When(x => x.EffectiveTo.HasValue);

            RuleForEach(x => x.Items).SetValidator(new UpdateFeeStructureItemDtoValidator());

            RuleFor(x => x.Items)
                .Must(items =>
                {
                    var active = items.Where(i => !i.IsDeleted).Select(i => i.FeeTypeId).ToList();
                    return active.Distinct().Count() == active.Count;
                })
                .WithMessage("Duplicate fee types are not allowed within the same fee structure.")
                .When(x => x.Items.Any());

            RuleFor(x => x.Items)
                .Must(items => items.Any(i => !i.IsDeleted))
                .WithMessage("A fee structure must have at least one active item.")
                .When(x => x.Items.Any());
        }
    }
}
