using FluentValidation;
using SchoolERP.Application.Features.ExamType.DTOs;

namespace SchoolERP.Application.Features.ExamType.Validators;

/// <summary>Validation rules for <see cref="CreateExamTypeDto"/>.</summary>
public class CreateExamTypeDtoValidator : AbstractValidator<CreateExamTypeDto>
{
    public CreateExamTypeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exam type name is required.")
            .MaximumLength(100);
    }
}
