using FluentValidation;
using SchoolERP.Application.Features.Authentication.DTOs;

namespace SchoolERP.Application.Features.Authentication.Validators;

/// <summary>Validation rules for <see cref="RefreshTokenRequestDto"/>.</summary>
public class RefreshTokenRequestDtoValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
