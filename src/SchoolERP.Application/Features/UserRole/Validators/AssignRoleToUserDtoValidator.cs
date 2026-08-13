using FluentValidation;
using SchoolERP.Application.Features.UserRole.DTOs;

namespace SchoolERP.Application.Features.UserRole.Validators;

/// <summary>Validation rules for <see cref="AssignRoleToUserDto"/>.</summary>
public class AssignRoleToUserDtoValidator : AbstractValidator<AssignRoleToUserDto>
{
    public AssignRoleToUserDtoValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("A valid user id is required.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0).WithMessage("A valid role id is required.");
    }
}
