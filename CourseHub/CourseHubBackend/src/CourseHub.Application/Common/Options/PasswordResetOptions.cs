namespace CourseHub.Application.Common.Options;

/// <summary>
/// Strongly typed password-reset configuration, bound from
/// "Authentication:PasswordReset". Contains no secrets.
/// </summary>
public class PasswordResetOptions
{
    public const string SectionName = "Authentication:PasswordReset";

    public int ExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Base URL the reset link is built from, e.g. "https://app.coursehub.example/reset-password".
    /// The raw token is appended as a query parameter. Points at a placeholder
    /// until a real frontend URL is configured.
    /// </summary>
    public string ResetUrlBase { get; set; } = "https://localhost/reset-password";
}
