using CourseHub.Application.Features.Permissions.Dtos;
using FluentValidation;

namespace CourseHub.Application.Features.Permissions.Validators;

public class AssignPermissionRequestValidator : AbstractValidator<AssignPermissionRequest>
{
    public AssignPermissionRequestValidator()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty()
            .MaximumLength(200);
    }
}
