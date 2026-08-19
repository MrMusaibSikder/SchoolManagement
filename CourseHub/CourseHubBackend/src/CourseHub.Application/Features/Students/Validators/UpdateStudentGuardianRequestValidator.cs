using CourseHub.Application.Features.Students.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Students.Validators;

public class UpdateStudentGuardianRequestValidator : AbstractValidator<UpdateStudentGuardianRequest>
{
    public UpdateStudentGuardianRequestValidator()
    {
        RuleFor(x => x.GuardianName)
            .MaximumLength(150);

        RuleFor(x => x.GuardianPhone)
            .MaximumLength(30);
    }
}
