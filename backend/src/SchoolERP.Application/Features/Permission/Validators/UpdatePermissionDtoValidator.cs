using FluentValidation;
using SchoolERP.Application.Features.Permission.DTOs;

namespace SchoolERP.Application.Features.Permission.Validators;

/// <summary>Validation rules for <see cref="UpdatePermissionDto"/>.</summary>
public class UpdatePermissionDtoValidator : AbstractValidator<UpdatePermissionDto>
{
    public UpdatePermissionDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("A valid permission id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required.")
            .MaximumLength(150)
            .Matches("^[A-Za-z]+\\.[A-Za-z]+$")
            .WithMessage("Permission name should follow the 'Resource.Action' convention, e.g. 'Student.View'.");
    }
}
