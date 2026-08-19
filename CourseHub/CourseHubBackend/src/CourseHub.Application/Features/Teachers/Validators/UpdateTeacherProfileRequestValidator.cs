using CourseHub.Application.Features.Teachers.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Teachers.Validators;

public class UpdateTeacherProfileRequestValidator : AbstractValidator<UpdateTeacherProfileRequest>
{
    public UpdateTeacherProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Bio)
            .MaximumLength(4000);
    }
}
