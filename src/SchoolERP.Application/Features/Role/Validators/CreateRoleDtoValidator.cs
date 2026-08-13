using FluentValidation;
using SchoolERP.Application.Features.Role.DTOs;

namespace SchoolERP.Application.Features.Role.Validators;

/// <summary>Validation rules for <see cref="CreateRoleDto"/>.</summary>
public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
{
    public CreateRoleDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
