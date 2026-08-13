using FluentValidation;
using SchoolERP.Application.Features.EmployeeAttendance.DTOs;

namespace SchoolERP.Application.Features.EmployeeAttendance.Validators;

/// <summary>Validation rules for <see cref="UpdateEmployeeAttendanceDto"/>.</summary>
public class UpdateEmployeeAttendanceDtoValidator : AbstractValidator<UpdateEmployeeAttendanceDto>
{
    public UpdateEmployeeAttendanceDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid attendance record id is required.");

        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("A valid employee id is required.");

        RuleFor(x => x.AttendanceDate)
            .NotEmpty().WithMessage("Attendance date is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("A valid attendance status is required.");

        RuleFor(x => x.CheckOut)
            .GreaterThanOrEqualTo(x => x.CheckIn)
            .When(x => x.CheckIn.HasValue && x.CheckOut.HasValue)
            .WithMessage("Check-out time cannot be earlier than check-in time.");
    }
}
