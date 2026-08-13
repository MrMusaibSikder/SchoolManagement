using FluentValidation;
using SchoolERP.Application.Features.GradeSetup.DTOs;

namespace SchoolERP.Application.Features.GradeSetup.Validators;

/// <summary>Validation rules for <see cref="CreateGradeSetupDto"/>.</summary>
public class CreateGradeSetupDtoValidator : AbstractValidator<CreateGradeSetupDto>
{
    public CreateGradeSetupDtoValidator()
    {
        RuleFor(x => x.AcademicYearId).GreaterThan(0).WithMessage("A valid academic year is required.");
        RuleFor(x => x.GradeName).NotEmpty().WithMessage("Grade name is required.").MaximumLength(10);
        RuleFor(x => x.GradePoint).InclusiveBetween(0, 5).WithMessage("Grade point must be between 0 and 5.");

        RuleFor(x => x.MinPercentage).InclusiveBetween(0, 100).WithMessage("Minimum percentage must be between 0 and 100.");
        RuleFor(x => x.MaxPercentage).InclusiveBetween(0, 100).WithMessage("Maximum percentage must be between 0 and 100.");
        RuleFor(x => x.MaxPercentage).GreaterThanOrEqualTo(x => x.MinPercentage).WithMessage("Maximum percentage cannot be less than minimum percentage.");

        RuleFor(x => x.MaxMarks).GreaterThanOrEqualTo(x => x.MinMarks).WithMessage("Maximum marks cannot be less than minimum marks.");

        RuleFor(x => x.DisplayOrder).GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");
    }
}
