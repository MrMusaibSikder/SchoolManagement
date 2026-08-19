using CourseHub.Application.Features.Teachers.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Teachers.Validators;

public class UpdateTeacherContactRequestValidator : AbstractValidator<UpdateTeacherContactRequest>
{
    public UpdateTeacherContactRequestValidator()
    {
        RuleFor(x => x.Phone)
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .MaximumLength(255)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
