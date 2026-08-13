using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeType.DTOs;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeType.Validators
{
    public class CreateFeeTypeDtoValidator : AbstractValidator<CreateFeeTypeDto>
    {
        public CreateFeeTypeDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
                .MustAsync(async (n, ct) => !await unitOfWork.FeeTypeRepository.NameExistsAsync(n, null, ct))
                .WithMessage("A fee type with this name already exists.");

            RuleFor(x => x.Code).NotEmpty().MaximumLength(20)
                .Matches("^[A-Z0-9_]+$").WithMessage("Code must be uppercase letters/numbers/underscore only.")
                .MustAsync(async (c, ct) => !await unitOfWork.FeeTypeRepository.CodeExistsAsync(c, null, ct))
                .WithMessage("A fee type with this code already exists.");

            RuleFor(x => x.Description).MaximumLength(300);

            RuleFor(x => x.FeeCategoryId)
                .GreaterThan(0)
                .MustAsync(async (id, ct) => await unitOfWork.FeeCategoryRepository.ExistsAsync(id, ct))
                .WithMessage("Selected fee category does not exist.");

            RuleFor(x => x.Frequency).IsInEnum();

            RuleFor(x => x.DefaultDueDayOfMonth)
                .NotNull().WithMessage("Due day of month is required for recurring (Monthly/Termly) fee types.")
                .When(x => x.Frequency != FeeFrequency.OneTime);

            RuleFor(x => x.DefaultDueDayOfMonth)
                .InclusiveBetween(1, 31)
                .When(x => x.DefaultDueDayOfMonth.HasValue)
                .WithMessage("Due day must be between 1 and 31.");

            RuleFor(x => x.DefaultDueDayOfMonth)
                .Null().WithMessage("One-time fee types should not have a due day of month.")
                .When(x => x.Frequency == FeeFrequency.OneTime);

            RuleFor(x => x.DefaultGracePeriodDays).GreaterThanOrEqualTo(0);
        }
    }
}
