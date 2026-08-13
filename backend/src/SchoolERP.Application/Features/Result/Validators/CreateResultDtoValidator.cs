using FluentValidation;
using SchoolERP.Application.Features.Result.DTOs;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Result.Validators;

/// <summary>Validation rules for <see cref="CreateResultDto"/>.</summary>
public class CreateResultDtoValidator : AbstractValidator<CreateResultDto>
{
    public CreateResultDtoValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0).WithMessage("A valid student is required.");
        RuleFor(x => x.ExamScheduleId).GreaterThan(0).WithMessage("A valid exam schedule is required.");
        RuleFor(x => x.TeacherId).GreaterThan(0).WithMessage("A valid teacher is required.");

        RuleFor(x => x.MarksObtained).GreaterThanOrEqualTo(0).WithMessage("Marks cannot be negative.");
        RuleFor(x => x.GraceMarks).GreaterThanOrEqualTo(0).WithMessage("Grace marks cannot be negative.");

        RuleFor(x => x.AttendanceStatus).IsInEnum().WithMessage("A valid attendance status is required.");

        RuleFor(x => x.MarksObtained)
            .Equal(0)
            .When(x => x.AttendanceStatus != MarkAttendanceStatus.Present)
            .WithMessage("Marks must be 0 when the student is not marked Present.");

        RuleFor(x => x.Remarks).MaximumLength(500);
    }
}
