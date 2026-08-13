using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeType.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeType.Validators
{
    public class UpdateFeeTypeDtoValidator : AbstractValidator<UpdateFeeTypeDto>
    {
        public UpdateFeeTypeDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
                .MustAsync(async (dto, n, ct) => !await unitOfWork.FeeTypeRepository.NameExistsAsync(n, dto.Id, ct))
                .WithMessage("A fee type with this name already exists.");

            RuleFor(x => x.Code).NotEmpty().MaximumLength(20)
                .Matches("^[A-Z0-9_]+$")
                .MustAsync(async (dto, c, ct) => !await unitOfWork.FeeTypeRepository.CodeExistsAsync(c, dto.Id, ct))
                .WithMessage("A fee type with this code already exists.");

            RuleFor(x => x.FeeCategoryId).GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.FeeCategoryRepository.ExistsAsync(id, ct))
                .WithMessage("Selected fee category does not exist.");

            RuleFor(x => x.Frequency).IsInEnum();

            RuleFor(x => x.DefaultDueDayOfMonth)
                .InclusiveBetween(1, 31)
                .When(x => x.DefaultDueDayOfMonth.HasValue);

            RuleFor(x => x.DefaultGracePeriodDays).GreaterThanOrEqualTo(0);
        }
    }
}
