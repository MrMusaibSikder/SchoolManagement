using FluentValidation;
using SchoolERP.Application.Features.EmployeeAttendance.DTOs;

namespace SchoolERP.Application.Features.EmployeeAttendance.Validators;

/// <summary>Validation rules for <see cref="CreateEmployeeAttendanceDto"/>.</summary>
public class CreateEmployeeAttendanceDtoValidator : AbstractValidator<CreateEmployeeAttendanceDto>
{
    public CreateEmployeeAttendanceDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("A valid employee id is required.");

        RuleFor(x => x.AttendanceDate)
            .NotEmpty().WithMessage("Attendance date is required.")
            .LessThanOrEqualTo(_ => DateTime.Today).WithMessage("Future attendance is not allowed.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid attendance status is required.");

        RuleFor(x => x.CheckOut)
            .GreaterThanOrEqualTo(x => x.CheckIn)
            .When(x => x.CheckIn.HasValue && x.CheckOut.HasValue)
            .WithMessage("Check-out time cannot be earlier than check-in time.");
    }
}
