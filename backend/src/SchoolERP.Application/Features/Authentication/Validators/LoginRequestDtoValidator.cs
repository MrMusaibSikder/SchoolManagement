using FluentValidation;
using SchoolERP.Application.Features.Authentication.DTOs;

namespace SchoolERP.Application.Features.Authentication.Validators;

/// <summary>Validation rules for <see cref="LoginRequestDto"/>.</summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().WithMessage("Username or email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
