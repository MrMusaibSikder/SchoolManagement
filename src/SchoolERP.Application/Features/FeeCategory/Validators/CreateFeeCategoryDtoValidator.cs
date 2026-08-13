using FluentValidation;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.FeeCategory.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.FeeCategory.Validators
{
    public class CreateFeeCategoryDtoValidator : AbstractValidator<CreateFeeCategoryDto>
    {
        public CreateFeeCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name is required.")
                .MaximumLength(100)
                .MustAsync(async (name, ct) => !await unitOfWork.FeeCategoryRepository.NameExistsAsync(name, null, ct))
                .WithMessage("A fee category with this name already exists.");

            RuleFor(x => x.Description)
                .MaximumLength(300);

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0);
        }
    }
}
