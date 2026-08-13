using FluentValidation;
using SchoolERP.Application.Features.Role.DTOs;

namespace SchoolERP.Application.Features.Role.Validators;

/// <summary>Validation rules for <see cref="UpdateRoleDto"/>.</summary>
public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid role id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
