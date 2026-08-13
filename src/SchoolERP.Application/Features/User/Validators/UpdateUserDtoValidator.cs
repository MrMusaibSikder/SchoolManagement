using FluentValidation;
using SchoolERP.Application.Features.User.DTOs;

namespace SchoolERP.Application.Features.User.Validators;

/// <summary>Validation rules for <see cref="UpdateUserDto"/>.</summary>
public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid user id is required.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(150);

        // Password is optional on update (empty means "keep existing password"),
        // but if supplied it must meet the same strength rules as on create.
        RuleFor(x => x.Password)
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .When(x => !string.IsNullOrWhiteSpace(x.Password));
    }
}
