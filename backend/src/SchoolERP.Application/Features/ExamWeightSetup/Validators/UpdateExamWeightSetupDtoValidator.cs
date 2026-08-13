using FluentValidation;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;

namespace SchoolERP.Application.Features.ExamWeightSetup.Validators;

/// <summary>Validation rules for <see cref="UpdateExamWeightSetupDto"/>.</summary>
public class UpdateExamWeightSetupDtoValidator : AbstractValidator<UpdateExamWeightSetupDto>
{
    public UpdateExamWeightSetupDtoValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("A valid setup id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Setup name is required.").MaximumLength(150);
    }
}
