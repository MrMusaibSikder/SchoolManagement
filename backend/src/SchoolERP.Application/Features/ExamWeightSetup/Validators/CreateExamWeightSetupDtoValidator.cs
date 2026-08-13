using FluentValidation;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;

namespace SchoolERP.Application.Features.ExamWeightSetup.Validators;

/// <summary>Validation rules for <see cref="CreateExamWeightSetupDto"/>.</summary>
public class CreateExamWeightSetupDtoValidator : AbstractValidator<CreateExamWeightSetupDto>
{
    public CreateExamWeightSetupDtoValidator()
    {
        RuleFor(x => x.AcademicYearId).GreaterThan(0).WithMessage("A valid academic year is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Setup name is required.").MaximumLength(150);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ExamId).GreaterThan(0).WithMessage("A valid exam is required.");
            item.RuleFor(x => x.WeightPercentage).GreaterThan(0).WithMessage("Weight must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Weight cannot exceed 100.");
        });

        RuleFor(x => x.Items)
            .Must(items => items.Select(i => i.ExamId).Distinct().Count() == items.Count)
            .WithMessage("Duplicate exam found in the weight items list.");
    }
}
