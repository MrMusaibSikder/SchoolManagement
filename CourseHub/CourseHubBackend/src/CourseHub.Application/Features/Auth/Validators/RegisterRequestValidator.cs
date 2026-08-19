using CourseHub.Application.Features.Auth.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private static readonly string[] AllowedRequestedRoles = { "Teacher", "Student" };

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Password and confirmation password do not match.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        // "Admin" is deliberately not in the allowed list — self-requesting
        // an admin role via a public endpoint would be a privilege
        // escalation. SuperAdmin only comes via SuperAdminCode.
        RuleFor(x => x.RequestedRole)
            .Must(role => role is null || AllowedRequestedRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            .WithMessage("RequestedRole must be 'Teacher' or 'Student' (or omitted).");
    }
}
