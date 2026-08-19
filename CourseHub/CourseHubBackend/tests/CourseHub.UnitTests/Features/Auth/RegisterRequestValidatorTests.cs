using CourseHub.Application.Features.Auth.Dtos;
using CourseHub.Application.Features.Auth.Validators;

namespace CourseHub.UnitTests.Features.Auth;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator = new();

    [Fact]
    public void Validate_ValidRequest_Passes()
    {
        var request = new RegisterRequest("user@example.com", "Password123", "Password123", "Jane", "Doe");

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PasswordTooShort_Fails()
    {
        var request = new RegisterRequest("user@example.com", "short", "short", "Jane", "Doe");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Fact]
    public void Validate_ConfirmPasswordMismatch_Fails()
    {
        var request = new RegisterRequest("user@example.com", "Password123", "Different123", "Jane", "Doe");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.ConfirmPassword));
    }

    [Fact]
    public void Validate_InvalidEmail_Fails()
    {
        var request = new RegisterRequest("not-an-email", "Password123", "Password123", "Jane", "Doe");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("Teacher")]
    [InlineData("Student")]
    [InlineData(null)]
    public void Validate_AllowedRequestedRole_Passes(string? requestedRole)
    {
        var request = new RegisterRequest("user@example.com", "Password123", "Password123", "Jane", "Doe", requestedRole);

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_AdminRequestedRole_Fails()
    {
        // Self-requesting Admin is a privilege-escalation risk — only
        // Teacher/Student are allowed here; Admin comes via role management.
        var request = new RegisterRequest("user@example.com", "Password123", "Password123", "Jane", "Doe", "Admin");

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
    }
}
