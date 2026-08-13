using FluentValidation;
using SchoolERP.Application.Features.Authentication.DTOs;

namespace SchoolERP.Application.Features.Authentication.Validators;

/// <summary>
/// Validation rules for <see cref="LogoutRequestDto"/>. The refresh token is
/// optional (omitting it revokes every active session for the current user), so
/// there is nothing to reject here beyond the model shape itself; this validator
/// exists mainly so Logout participates in the same validation pipeline as every
/// other authentication endpoint.
/// </summary>
public class LogoutRequestDtoValidator : AbstractValidator<LogoutRequestDto>
{
    public LogoutRequestDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .MaximumLength(512)
            .When(x => x.RefreshToken is not null);
    }
}
