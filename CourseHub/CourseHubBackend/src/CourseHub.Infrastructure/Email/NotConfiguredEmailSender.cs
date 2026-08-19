using CourseHub.Application.Common.Interfaces;

namespace CourseHub.Infrastructure.Email;

/// <summary>
/// Registered outside Development so that a missing real email provider
/// fails loudly and immediately, rather than silently pretending to send
/// (or worse, silently logging reset links) in production. Replace this
/// registration with a real IEmailSender implementation (SendGrid, Amazon
/// SES, SMTP, etc.) before deploying anywhere real users can register.
/// </summary>
public class NotConfiguredEmailSender : IEmailSender
{
    public Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default)
    {
        throw new InvalidOperationException(
            "No production email provider is configured. Implement IEmailSender with a real " +
            "provider and register it in place of NotConfiguredEmailSender before enabling " +
            "password reset outside Development.");
    }
}
