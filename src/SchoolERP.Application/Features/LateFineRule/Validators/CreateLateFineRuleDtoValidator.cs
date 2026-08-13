using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.LateFineRule.DTOs;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.LateFineRule.Validators
{
    public class CreateLateFineRuleDtoValidator : AbstractValidator<CreateLateFineRuleDto>
    {
        public CreateLateFineRuleDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.AcademicYearId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.AcademicYearRepository.ExistsAsync(id, ct))
                .WithMessage("Selected academic year does not exist.");

            RuleFor(x => x.FeeTypeId)
                .MustAsync(async (id, ct) => id == null || await unitOfWork.FeeTypeRepository.ExistsAsync(id.Value, ct))
                .WithMessage("Selected fee type does not exist.");

            RuleFor(x => x.Type).IsInEnum();

            RuleFor(x => x.Amount).GreaterThan(0);

            RuleFor(x => x.Amount)
                .InclusiveBetween(0.01m, 100)
                .WithMessage("Percentage fine amount must be between 0 and 100.")
                .When(x => x.Type == FineType.Percentage);

            RuleFor(x => x.GracePeriodDays).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MaxFineAmount).GreaterThan(0).When(x => x.MaxFineAmount.HasValue);

            RuleFor(x => x)
                .MustAsync(async (dto, ct) =>
                {
                    var existing = await unitOfWork.LateFineRuleRepository.GetApplicableRuleAsync(
                        dto.AcademicYearId, dto.FeeTypeId ?? 0, ct);
                    return existing == null;
                })
                .WithMessage("A late fine rule already exists for this academic year/fee type combination.")
                .WithName("FeeTypeId");
        }
    }
}
