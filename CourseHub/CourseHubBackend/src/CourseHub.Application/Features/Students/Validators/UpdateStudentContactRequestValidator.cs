using CourseHub.Application.Features.Students.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Students.Validators;

public class UpdateStudentContactRequestValidator : AbstractValidator<UpdateStudentContactRequest>
{
    public UpdateStudentContactRequestValidator()
    {
        RuleFor(x => x.Phone)
            .MaximumLength(30);

        RuleFor(x => x.Email)
            .MaximumLength(255)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(300);
    }
}
