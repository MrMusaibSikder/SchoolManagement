using FluentValidation;
using SchoolERP.Application.Features.ExamType.DTOs;

namespace SchoolERP.Application.Features.ExamType.Validators;

/// <summary>Validation rules for <see cref="UpdateExamTypeDto"/>.</summary>
public class UpdateExamTypeDtoValidator : AbstractValidator<UpdateExamTypeDto>
{
    public UpdateExamTypeDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid exam type id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exam type name is required.")
            .MaximumLength(100);
    }
}
