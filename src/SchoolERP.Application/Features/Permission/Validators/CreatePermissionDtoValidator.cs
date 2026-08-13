using FluentValidation;
using SchoolERP.Application.Features.Permission.DTOs;

namespace SchoolERP.Application.Features.Permission.Validators;

/// <summary>Validation rules for <see cref="CreatePermissionDto"/>.</summary>
public class CreatePermissionDtoValidator : AbstractValidator<CreatePermissionDto>
{
    public CreatePermissionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required.")
            .MaximumLength(150)
            .Matches("^[A-Za-z]+\\.[A-Za-z]+$")
            .WithMessage("Permission name should follow the 'Resource.Action' convention, e.g. 'Student.View'.");
    }
}
