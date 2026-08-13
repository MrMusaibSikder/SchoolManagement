using FluentValidation;
using SchoolERP.Application.Features.Result.DTOs;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Result.Validators;

/// <summary>Validation rules for <see cref="BulkMarkEntryDto"/>.</summary>
public class BulkMarkEntryDtoValidator : AbstractValidator<BulkMarkEntryDto>
{
    public BulkMarkEntryDtoValidator()
    {
        RuleFor(x => x.ExamScheduleId).GreaterThan(0).WithMessage("A valid exam schedule is required.");
        RuleFor(x => x.TeacherId).GreaterThan(0).WithMessage("A valid teacher is required.");

        RuleFor(x => x.Entries).NotEmpty().WithMessage("At least one mark entry is required.");

        RuleForEach(x => x.Entries).ChildRules(entry =>
        {
            entry.RuleFor(x => x.StudentId).GreaterThan(0).WithMessage("A valid student is required.");
            entry.RuleFor(x => x.MarksObtained).GreaterThanOrEqualTo(0).WithMessage("Marks cannot be negative.");
            entry.RuleFor(x => x.GraceMarks).GreaterThanOrEqualTo(0).WithMessage("Grace marks cannot be negative.");
            entry.RuleFor(x => x.AttendanceStatus).IsInEnum().WithMessage("A valid attendance status is required.");
            entry.RuleFor(x => x.MarksObtained)
                .Equal(0)
                .When(x => x.AttendanceStatus != MarkAttendanceStatus.Present)
                .WithMessage("Marks must be 0 when the student is not marked Present.");
            entry.RuleFor(x => x.Remarks).MaximumLength(500);
        });

        RuleFor(x => x.Entries)
            .Must(entries => entries.Select(e => e.StudentId).Distinct().Count() == entries.Count)
            .WithMessage("Duplicate student found in the mark entry list.");
    }
}
