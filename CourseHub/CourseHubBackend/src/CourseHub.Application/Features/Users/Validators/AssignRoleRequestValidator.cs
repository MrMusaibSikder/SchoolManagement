using CourseHub.Application.Features.Users.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Users.Validators;

public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
{
    public AssignRoleRequestValidator()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .MaximumLength(100);
    }
}
