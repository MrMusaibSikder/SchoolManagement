using FluentValidation;
using SchoolERP.Application.Features.ExamSchedule.DTOs;

namespace SchoolERP.Application.Features.ExamSchedule.Validators;

/// <summary>Validation rules for <see cref="CreateExamScheduleDto"/>.</summary>
public class CreateExamScheduleDtoValidator : AbstractValidator<CreateExamScheduleDto>
{
    public CreateExamScheduleDtoValidator()
    {
        RuleFor(x => x.ExamId)
            .GreaterThan(0).WithMessage("A valid exam is required.");

        RuleFor(x => x.ClassId)
            .GreaterThan(0).WithMessage("A valid class is required.");

        RuleFor(x => x.SubjectId)
            .GreaterThan(0).WithMessage("A valid subject is required.");

        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("Exam date is required.");

        RuleFor(x => x.FullMarks)
            .GreaterThan(0).WithMessage("Full marks must be greater than 0.");

        RuleFor(x => x.PassMarks)
            .GreaterThan(0).WithMessage("Pass marks must be greater than 0.")
            .LessThan(x => x.FullMarks).WithMessage("Pass marks must be less than full marks.");
    }
}
