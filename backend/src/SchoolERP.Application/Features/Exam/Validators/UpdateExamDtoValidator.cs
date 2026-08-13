using FluentValidation;
using SchoolERP.Application.Features.Exam.DTOs;

namespace SchoolERP.Application.Features.Exam.Validators;

/// <summary>Validation rules for <see cref="UpdateExamDto"/>.</summary>
public class UpdateExamDtoValidator : AbstractValidator<UpdateExamDto>
{
    public UpdateExamDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid exam id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exam name is required.")
            .MaximumLength(100);

        RuleFor(x => x.ExamTypeId)
            .GreaterThan(0).WithMessage("A valid exam type is required.");

        RuleFor(x => x.AcademicYearId)
            .GreaterThan(0).WithMessage("A valid academic year is required.");
    }
}
