using FluentValidation;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;

namespace SchoolERP.Application.Features.ExamWeightSetup.Validators;

/// <summary>Validation rules for <see cref="AddExamWeightItemDto"/>.</summary>
public class AddExamWeightItemDtoValidator : AbstractValidator<AddExamWeightItemDto>
{
    public AddExamWeightItemDtoValidator()
    {
        RuleFor(x => x.ExamWeightSetupId).GreaterThan(0).WithMessage("A valid weight setup is required.");
        RuleFor(x => x.ExamId).GreaterThan(0).WithMessage("A valid exam is required.");
        RuleFor(x => x.WeightPercentage).GreaterThan(0).WithMessage("Weight must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Weight cannot exceed 100.");
    }
}
