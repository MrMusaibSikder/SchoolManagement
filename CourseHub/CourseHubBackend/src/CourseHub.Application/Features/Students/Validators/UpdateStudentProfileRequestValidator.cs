using CourseHub.Application.Features.Students.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Students.Validators;

public class UpdateStudentProfileRequestValidator : AbstractValidator<UpdateStudentProfileRequest>
{
    public UpdateStudentProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DateOfBirth)
            .Must(dob => !dob.HasValue || dob.Value < DateTime.UtcNow)
            .WithMessage("DateOfBirth must be in the past.");
    }
}
