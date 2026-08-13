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
    public class UpdateFeeCategoryDtoValidator : AbstractValidator<UpdateFeeCategoryDto>
    {
        public UpdateFeeCategoryDtoValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(x => x.Id).GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .MustAsync(async (dto, name, ct) => !await unitOfWork.FeeCategoryRepository.NameExistsAsync(name, dto.Id, ct))
                .WithMessage("A fee category with this name already exists.");

            RuleFor(x => x.Description).MaximumLength(300);
            RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0);
        }
    }
}
