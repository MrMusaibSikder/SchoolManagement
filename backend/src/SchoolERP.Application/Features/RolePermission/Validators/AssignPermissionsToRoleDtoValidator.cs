using FluentValidation;
using SchoolERP.Application.Features.RolePermission.DTOs;

namespace SchoolERP.Application.Features.RolePermission.Validators;

/// <summary>Validation rules for <see cref="AssignPermissionsToRoleDto"/>.</summary>
public class AssignPermissionsToRoleDtoValidator : AbstractValidator<AssignPermissionsToRoleDto>
{
    public AssignPermissionsToRoleDtoValidator()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("A valid role id is required.");

        RuleFor(x => x.PermissionIds)
            .NotEmpty().WithMessage("At least one permission id must be supplied.");

        RuleForEach(x => x.PermissionIds)
            .GreaterThan(0).WithMessage("Permission ids must be valid.");
    }
}
