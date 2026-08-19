using CourseHub.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CourseHub.Infrastructure.Email;

/// <summary>
/// DEVELOPMENT-ONLY email sender. No real email provider is wired up yet
/// (see IEmailSender doc comment). Registered only when
/// IHostEnvironment.IsDevelopment() is true (see CourseHub.API/Program.cs)
/// so the password-reset flow is testable locally without SMTP/a real
/// provider.
///
/// The reset link is logged at Debug level, gated behind IsEnabled, so it
/// does not appear unless a developer has explicitly turned on Debug
/// logging — this is a deliberate, narrow, documented exception to "never
/// log tokens," scoped to local development only. Never enable this
/// sender, or log at this verbosity, in production.
/// </summary>
public class DevelopmentEmailSender : IEmailSender
{
    private readonly ILogger<DevelopmentEmailSender> _logger;

    public DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "[DEV EMAIL - NOT SENT] Password reset link for {Email}: {ResetLink}",
                toEmail,
                resetLink);
        }

        return Task.CompletedTask;
    }
}
