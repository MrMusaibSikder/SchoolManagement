using FluentValidation;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;

namespace SchoolERP.Application.Features.ExamWeightSetup.Validators;

/// <summary>Validation rules for <see cref="UpdateExamWeightItemDto"/>.</summary>
public class UpdateExamWeightItemDtoValidator : AbstractValidator<UpdateExamWeightItemDto>
{
    public UpdateExamWeightItemDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("A valid weight item id is required.");
        RuleFor(x => x.WeightPercentage).GreaterThan(0).WithMessage("Weight must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Weight cannot exceed 100.");
    }
}
