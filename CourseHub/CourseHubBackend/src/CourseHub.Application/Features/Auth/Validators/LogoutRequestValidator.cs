using CourseHub.Application.Features.Auth.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Auth.Validators;

public class LogoutRequestValidator : AbstractValidator<LogoutRequest>
{
    public LogoutRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
