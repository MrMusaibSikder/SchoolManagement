using FluentValidation;
using SchoolERP.Application.Features.Authentication.DTOs;

namespace SchoolERP.Application.Features.Authentication.Validators;

/// <summary>Validation rules for <see cref="ForgotPasswordDto"/>.</summary>
public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
    }
}
