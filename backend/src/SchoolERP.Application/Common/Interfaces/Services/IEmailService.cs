namespace SchoolERP.Application.Common.Interfaces.Services;

/// <summary>
/// Sends transactional emails (password reset links, notifications, etc).
/// Kept as an abstraction so the delivery mechanism (SMTP, SendGrid, ...) can be
/// swapped without touching business logic in services.
/// </summary>
public interface IEmailService
{
    /// <summary>Sends a plain-text or HTML email to a single recipient.</summary>
    Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
}
