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
    public class CreateFeeStructureDtoValidator : AbstractValidator<CreateFeeStructureDto>
    {
        public CreateFeeStructureDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Description).MaximumLength(500);

            RuleFor(x => x.AcademicYearId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.AcademicYearRepository.ExistsAsync(id, ct))
                .WithMessage("Selected academic year does not exist.");

            RuleFor(x => x.SchoolClassId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.SchoolClassRepository.ExistsAsync(id, ct))
                .WithMessage("Selected school class does not exist.");

            RuleFor(x => x.SectionId)
                .MustAsync(async (id, ct) => id == null || await unitOfWork.SectionRepository.ExistsAsync(id.Value, ct))
                .WithMessage("Selected section does not exist.");

            RuleFor(x => x.EffectiveFrom).NotEmpty();

            RuleFor(x => x.EffectiveTo)
                .GreaterThan(x => x.EffectiveFrom)
                .When(x => x.EffectiveTo.HasValue)
                .WithMessage("Effective-to date must be after effective-from date.");

            RuleFor(x => x)
                .MustAsync(async (dto, ct) => !await unitOfWork.FeeStructureRepository.ExistsForClassSectionYearAsync(
                    dto.SchoolClassId, dto.SectionId, dto.AcademicYearId, null, ct))
                .WithMessage("A fee structure already exists for this class/section/academic year combination.")
                .WithName("SchoolClassId");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one fee structure item is required.");

            RuleForEach(x => x.Items).SetValidator(new CreateFeeStructureItemDtoValidator());

            RuleFor(x => x.Items)
                .Must(items => items.Select(i => i.FeeTypeId).Distinct().Count() == items.Count)
                .WithMessage("Duplicate fee types are not allowed within the same fee structure.")
                .When(x => x.Items.Any());

            RuleFor(x => x.ClonedFromId)
                .MustAsync(async (id, ct) => id == null || await unitOfWork.FeeStructureRepository.ExistsAsync(id.Value, ct))
                .WithMessage("Referenced 'cloned from' structure does not exist.");
        }
    }
}
